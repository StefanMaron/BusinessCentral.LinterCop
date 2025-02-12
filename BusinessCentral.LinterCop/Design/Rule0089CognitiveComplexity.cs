using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using System.Collections.Concurrent;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

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

            // The 'else if' increment causes a problem
            // In the AL Language 'else if' is an 'else" keyword followed by an 'if' node (not a single 'elsif' node).
            // If we increment for both 'else' and 'if' kinds the number will be too high.
            // So we'll increment for 'else' nodes not followed by an 'if' and rely on the 'if' to increment 'else if' statements.
            if (node is IfStatementSyntax ifStatement)
            {
                if (!IsGuardClause(node))
                {
                    // Increment for the 'if' condition (+1 + nesting level)
                    complexity += 1 + nestingLevel;
                    RaiseDEBUGDiagnostic(context, node, node.SpanStart, node.Kind, nestingLevel);
                }

                // Push the 'then' block with increased nesting
                if (ifStatement.Statement != null)
                    stack.Push((ifStatement.Statement, nestingLevel + 1));

                // Handle the 'else' ElseStatement
                if (ifStatement.ElseStatement is not null)
                {
                    if (ifStatement.ElseStatement is IfStatementSyntax)
                    {
                        // ELSE IF: Do not increment and no nesting penalty
                        // Rely on the 'if' to increment
                        stack.Push((ifStatement.ElseStatement, nestingLevel));
                    }
                    else
                    {
                        // ELSE (not followed by IF): Increment by 1, No(!) nesting penalty
                        complexity += 1;
                        RaiseDEBUGDiagnostic(context, node, ifStatement.ElseKeywordToken.SpanStart, SyntaxKind.ElseKeyword, nestingLevel);
                        stack.Push((ifStatement.ElseStatement, nestingLevel));
                    }
                }

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
        if (node is IfStatementSyntax ifStatement)
        {
#if !LessThenFall2025 // TODO: Change to LessThenSpring2025 when AL version 15.0 is no longer in Pre-Release
            return ifStatement.Statement is ExitStatementSyntax or ContinueStatementSyntax;
#else
            return ifStatement.Statement is ExitStatementSyntax;
#endif
        }
        return false;
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