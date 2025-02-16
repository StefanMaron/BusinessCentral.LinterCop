using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0089CognitiveComplexity : DiagnosticAnalyzer
{
    private int complexityThreshold;

    // Flow-Breaking Structures: These disrupt the linear execution of the code.
    // Each occurrence of these structures adds +1 complexity to the score.
    private static readonly HashSet<SyntaxKind> flowBreakingKinds = new()
    {
        SyntaxKind.IfStatement,
        SyntaxKind.CaseStatement,
        SyntaxKind.ForStatement,
        SyntaxKind.ForEachStatement,
        SyntaxKind.WhileStatement,
        SyntaxKind.RepeatStatement,
#if !LessThenFall2024
        SyntaxKind.ConditionalExpression // Ternary operator
#endif
    };

    // Nested Structures: These introduce additional cognitive load due to nesting.
    // Unlike flow-breaking structures that always add complexity, nested structures only add an extra penalty when nested inside another structure.
    // Currently there's no difference between the Flow-Breaking Structures and Nested Structures in the AL Language.
    // For example in C# nestedStructures could contain try-catch-finally
    private static readonly HashSet<SyntaxKind> nestedStructures = new()
    {
        SyntaxKind.IfStatement,
        SyntaxKind.CaseStatement,
        SyntaxKind.ForStatement,
        SyntaxKind.ForEachStatement,
        SyntaxKind.WhileStatement,
        SyntaxKind.RepeatStatement,
#if !LessThenFall2024
        SyntaxKind.ConditionalExpression // Ternary operator
#endif
    };

    // This HashSet defines specific identifiers that, in certain cases, restrict whether a statement qualifies as a guard clause.
    // Some exit commands (e.g., "Break", "Skip", "Quit") are only considered guard clauses if they are called on these identifiers.
    private static readonly HashSet<string> guardClauseIdentifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        "CurrReport",
        "CurrXMLport"
    };

    // This HashSet defines commands that act as guard clause exits, meaning they immediately alter the flow of execution.
    // These commands are typically used in scenarios where a function, loop, or process needs to be stopped or skipped under certain conditions.
    // However, "Exit" is not included in this set, as we can get the ExitStatementSyntax type directly on the Statement of the IfStatementSyntax
    private static readonly HashSet<string> guardClauseExitCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "Break",
        "Continue",
        "Error",
        "Quit",
        "Skip"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DiagnosticDescriptors.Rule0089CognitiveComplexity,
            DiagnosticDescriptors.Rule0089DEBUGCognitiveComplexity,
            DiagnosticDescriptors.Rule0090CognitiveComplexity);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(compilationContext =>
        {
            this.complexityThreshold = LoadCognitiveComplexityThreshold(compilationContext.Compilation);

            compilationContext.RegisterCodeBlockAction(codeBlockContext =>
            {
                AnalyzeCognitiveComplexity(codeBlockContext);
            });
        });
    }

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

        int complexity = CalculateCognitiveComplexity(context, bodyNode);
        if (complexity >= complexityThreshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0090CognitiveComplexity,
                context.OwningSymbol.GetLocation(),
                complexity,
                complexityThreshold));
        }

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0089CognitiveComplexity,
            context.OwningSymbol.GetLocation(),
            complexity,
            complexityThreshold));
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
        if (node.Kind is SyntaxKind.LogicalAndExpression or SyntaxKind.LogicalOrExpression or SyntaxKind.LogicalXorExpression)
            return node.Parent.Kind != node.Kind;

        return false;
    }

    private bool IsNestedStructure(SyntaxNode node) =>
        nestedStructures.Contains(node.Kind);

    private bool IsGuardClause(SyntaxNode node)
    {
        return node switch
        {
            // if not <condition> then exit;
            IfStatementSyntax { Statement: ExitStatementSyntax } => true,

            IfStatementSyntax { Statement: ExpressionStatementSyntax { Expression: CodeExpressionSyntax codeExpression } }
                => IsGuardExpression(codeExpression),
            _ => false
        };
    }

    private bool IsGuardExpression(CodeExpressionSyntax codeExpression)
    {
        return codeExpression switch
        {
            // if not <condition> then continue;
            IdentifierNameSyntax identifier when identifier.GetIdentifierOrLiteralValue() is { } value
                => guardClauseExitCommands.Contains(value),

            InvocationExpressionSyntax invocation => IsGuardInvocation(invocation),
            _ => false
        };
    }

    private bool IsGuardInvocation(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => IsGuardCommand(memberAccess),

            // if not <condition> then error;
            IdentifierNameSyntax identifier when identifier.GetIdentifierOrLiteralValue() is { } value
                => guardClauseExitCommands.Contains(value),
            _ => false
        };
    }

    private bool IsGuardCommand(MemberAccessExpressionSyntax memberAccess)
    {
        if (memberAccess.Expression.GetIdentifierOrLiteralValue() is not { } identifierValue)
            return false;

        // if not <condition> then CurrReport.Break() or .Skip() or .Quit();
        return guardClauseIdentifiers.Contains(identifierValue) &&
               guardClauseExitCommands.Contains(memberAccess.GetNameStringValue() ?? string.Empty);
    }

    private int LoadCognitiveComplexityThreshold(Compilation compilation)
    {
        string? directoryPath = compilation.FileSystem?.GetDirectoryPath();
        LinterSettings.Create(directoryPath);
        return LinterSettings.instance?.cognitiveComplexityThreshold ?? 15;
    }

    private void RaiseDEBUGDiagnostic(CodeBlockAnalysisContext context, SyntaxNode node, int SpanStart, SyntaxKind nodeKind, int nestingLevel)
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