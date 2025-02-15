using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.ComponentModel;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0089CognitiveComplexity : DiagnosticAnalyzer
{
    private static readonly ConcurrentDictionary<Compilation, int> thresholdCache = new();

    private static readonly HashSet<SyntaxKind> flowBreakingKinds = new()
    {
        SyntaxKind.IfStatement,
        SyntaxKind.CaseStatement,
        SyntaxKind.ForStatement,
        SyntaxKind.ForEachStatement,
        SyntaxKind.WhileStatement,
        SyntaxKind.RepeatStatement,
#if !LessThenFall2024
        SyntaxKind.ConditionalExpression
#endif
    };

    private static readonly HashSet<SyntaxKind> nestedStructures = new()
    {
        SyntaxKind.IfStatement,
        SyntaxKind.CaseStatement,
        SyntaxKind.ForStatement,
        SyntaxKind.ForEachStatement,
        SyntaxKind.WhileStatement,
        SyntaxKind.RepeatStatement,
#if !LessThenFall2024
        SyntaxKind.ConditionalExpression
#endif
    };

    private static readonly HashSet<string> guardClauseIdentifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        "CurrReport",
        "CurrXMLport"
    };

    private static readonly HashSet<string> guardClauseExitCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "Break",
        "Skip",
        "Quit"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DiagnosticDescriptors.Rule0089CognitiveComplexity,
            DiagnosticDescriptors.Rule0089DEBUGCognitiveComplexity,
            DiagnosticDescriptors.Rule0090CognitiveComplexity);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterCodeBlockAction(new Action<CodeBlockAnalysisContext>(this.AnalyzeCognitiveComplexity));

    private void AnalyzeCognitiveComplexity(CodeBlockAnalysisContext context)
    {
        if (context.IsObsoletePendingOrRemoved())
            return;

        if ((context.CodeBlock.Kind != SyntaxKind.MethodDeclaration) &&
            (context.CodeBlock.Kind != SyntaxKind.TriggerDeclaration))
            return;

        var containingObjectTypeSymbol = context.OwningSymbol.GetContainingObjectTypeSymbol();
        if (containingObjectTypeSymbol.NavTypeKind == NavTypeKind.Interface ||
            containingObjectTypeSymbol.NavTypeKind == NavTypeKind.ControlAddIn)
            return;

        SyntaxNode? bodyNode = context.CodeBlock.Kind == SyntaxKind.MethodDeclaration
            ? (context.CodeBlock as MethodDeclarationSyntax)?.Body
            : (context.CodeBlock as TriggerDeclarationSyntax)?.Body;
        if (bodyNode is null)
            return;

        int cognitiveComplexityThreshold = GetCognitiveComplexityThreshold(context);
        int complexity = CalculateCognitiveComplexity(context, bodyNode);

        if (complexity >= cognitiveComplexityThreshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0090CognitiveComplexity,
                context.OwningSymbol.GetLocation(),
                complexity,
                cognitiveComplexityThreshold));
        }

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0089CognitiveComplexity,
            context.OwningSymbol.GetLocation(),
            complexity,
            cognitiveComplexityThreshold));
    }

    private int CalculateCognitiveComplexity(CodeBlockAnalysisContext context, SyntaxNode root)
    {
        int complexity = 0;
        var stack = new Stack<(SyntaxNode node, int nestingLevel)>();
        stack.Push((root, 0));

        while (stack.Count > 0)
        {
            var (node, nestingLevel) = stack.Pop();

            if (node.IsKind(SyntaxKind.IfStatement))
            {
                ProcessIfStatement(context, ref stack, node, ref complexity, ref nestingLevel);
                continue; // Skip further processing for this IF node
            }

            if (IsFlowBreakingStructure(node) && !IsGuardClause(node))
            {
                complexity += 1 + nestingLevel;
                RaiseDEBUGDiagnostic(context, node, node.SpanStart, node.Kind, nestingLevel);

                if (IsNestedStructure(node))
                    nestingLevel++;
            }

            foreach (var child in node.ChildNodes())
            {
                stack.Push((child, nestingLevel));
            }
        }

        return complexity;
    }

    // The 'else if' increment causes a problem
    // In the AL Language 'else if' is an 'else" keyword followed by an 'if' node (not a single 'elsif' node).
    // If we increment for both 'else' and 'if' kinds the number will be too high.
    // So we'll increment for 'else' nodes not followed by an 'if' and rely on the 'if' to increment 'else if' statements.
    private void ProcessIfStatement(CodeBlockAnalysisContext context, ref Stack<(SyntaxNode, int)> stack, SyntaxNode node, ref int complexity, ref int nestingLevel)
    {
        if (node is not IfStatementSyntax ifStatement)
            return;

        if (!IsGuardClause(node))
        {
            // Increment for the 'if' condition (+1 + nesting level)
            complexity += 1 + nestingLevel;
            RaiseDEBUGDiagnostic(context, node, node.SpanStart, node.Kind, nestingLevel);
        }

        // Push the 'then' block with increased nesting
        if (ifStatement.Statement is not null)
            stack.Push((ifStatement.Statement, nestingLevel + 1));

        // Handle 'else' statement logic from 'if' statement
        if (ifStatement.ElseStatement is not null)
        {
            if (ifStatement.ElseStatement is not IfStatementSyntax)
            {
                // 'else' not followed by 'if': Increment +1 (no nesting penalty)
                complexity += 1;
                RaiseDEBUGDiagnostic(context, node, ifStatement.ElseKeywordToken.SpanStart, SyntaxKind.ElseKeyword, nestingLevel);
            }

            // 'else if': Do not increment and no nesting penalty (rely on the 'if' statement to handle increment)
            stack.Push((ifStatement.ElseStatement, nestingLevel));
        }
    }

    private bool IsFlowBreakingStructure(SyntaxNode node)
    {
        // Fast path for common flow-breaking structures
        if (flowBreakingKinds.Contains(node.Kind))
            return true;

        // Apply Cognitive Complexity discount for consecutive logical operators
        if (node.Kind is SyntaxKind.LogicalAndExpression or SyntaxKind.LogicalOrExpression)
            return node.Parent.Kind != node.Kind;

        return false;
    }

    private bool IsNestedStructure(SyntaxNode node) =>
        nestedStructures.Contains(node.Kind);

    private bool IsGuardClause(SyntaxNode node)
    {
        if (node is not IfStatementSyntax { Statement: ExpressionStatementSyntax { Expression: CodeExpressionSyntax codeExpression } } ifStatement)
            return node is IfStatementSyntax { Statement: ExitStatementSyntax };

        if (codeExpression is IdentifierNameSyntax identifier)
        {
            return SemanticFacts.IsSameName(identifier.GetIdentifierOrLiteralValue() ?? string.Empty, "Continue");
        }

        if (codeExpression is InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                return IsGuardCommand(memberAccess);
            }
            if (invocation.Expression is IdentifierNameSyntax expression)
            {
                return SemanticFacts.IsSameName(expression.GetIdentifierOrLiteralValue() ?? string.Empty, "Error");
            }
        }
        return false;
    }

    private bool IsGuardCommand(MemberAccessExpressionSyntax memberAccess)
    {
        var identifier = memberAccess.Expression.GetIdentifierOrLiteralValue() ?? string.Empty;
        return identifier is not null && guardClauseIdentifiers.Contains(identifier) &&
               guardClauseExitCommands.Contains(memberAccess.GetNameStringValue() ?? string.Empty);
    }

    private int GetCognitiveComplexityThreshold(CodeBlockAnalysisContext context)
    {
        var compilation = context.SemanticModel.Compilation;

        if (!thresholdCache.TryGetValue(compilation, out int threshold))
        {
            LinterSettings.Create(context.SemanticModel.Compilation.FileSystem?.GetDirectoryPath());
            threshold = LinterSettings.instance?.cognitiveComplexityThreshold ?? 15;
            thresholdCache[compilation] = threshold;
        }

        return threshold;
    }

    private static void RaiseDEBUGDiagnostic(CodeBlockAnalysisContext context, SyntaxNode node, int SpanStart, SyntaxKind nodeKind, int nestingLevel)
    {
        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0089DEBUGCognitiveComplexity,
            Location.Create(
                node.GetLocation().SourceTree!,
                new TextSpan(SpanStart, 1)),
            nodeKind,
            1 + nestingLevel,
            nestingLevel));
    }
}