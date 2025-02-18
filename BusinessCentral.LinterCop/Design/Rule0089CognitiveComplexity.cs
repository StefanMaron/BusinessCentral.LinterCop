using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0089CognitiveComplexity : DiagnosticAnalyzer
{
    private int complexityThreshold;

    private Dictionary<IMethodSymbol, List<IMethodSymbol>> GlobalMethodInvocationGraph = new();

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

    private static readonly HashSet<string> eventPublisherDecoratorNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "BusinessEvent",
        "IntegrationEvent",
        "ExternalBusinessEvent"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DiagnosticDescriptors.Rule0089CognitiveComplexity,
            DiagnosticDescriptors.Rule0089IncrementCognitiveComplexity,
            DiagnosticDescriptors.Rule0090CognitiveComplexity);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(compilationContext =>
        {
            LoadCognitiveComplexityThreshold(compilationContext.Compilation);
            BuildGlobalMethodInvocationGraph(compilationContext);

            compilationContext.RegisterCodeBlockAction(codeBlockContext =>
            {
                AnalyzeCognitiveComplexity(codeBlockContext);
            });
        });
    }

    private void AnalyzeCognitiveComplexity(CodeBlockAnalysisContext context)
    {
        if (context.IsObsoletePendingOrRemoved() || context.CodeBlock is not MethodOrTriggerDeclarationSyntax methodOrTrigger)
            return;

        var containingObjectTypeSymbol = context.OwningSymbol.GetContainingObjectTypeSymbol();
        if (containingObjectTypeSymbol.NavTypeKind == NavTypeKind.Interface ||
            containingObjectTypeSymbol.NavTypeKind == NavTypeKind.ControlAddIn)
            return;

        if (methodOrTrigger.Body is null ||
            methodOrTrigger.Body.Statements.Count == 0 &&
            methodOrTrigger.Attributes.Any(attr => eventPublisherDecoratorNames.Contains(attr.GetIdentifierOrLiteralValue() ?? string.Empty)))
            return;

        int complexity = CalculateCognitiveComplexity(context, methodOrTrigger.Body);
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
                RaiseIncrementDiagnostic(context, GetKeywordLocation(node, node.SpanStart), node.Kind.ToString(), nestingLevel);

                if (IsNestedStructure(node))
                    nestingLevel++;
            }

            foreach (var child in node.ChildNodes())
            {
                stack.Push((child, nestingLevel));
            }
        }

        if (context.CodeBlock.IsKind(SyntaxKind.MethodDeclaration))
        {
            complexity += CalculateRecursionComplexity(context, root);
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
            // Increment for the 'if' statement
            complexity += 1 + nestingLevel;
            RaiseIncrementDiagnostic(context, GetKeywordLocation(node, node.SpanStart), node.Kind.ToString(), nestingLevel);
        }

        // Push the condition of the 'if' statement back to the stack
        stack.Push((ifStatement.Condition, nestingLevel));

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
                RaiseIncrementDiagnostic(context, ifStatement.ElseKeywordToken.GetLocation(), "ElseStatement", nestingLevel);
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

    #region Recursion

    private int CalculateRecursionComplexity(CodeBlockAnalysisContext context, SyntaxNode root)
    {
        int increment = 0;
        var visited = new HashSet<IMethodSymbol>();

        if (context.OwningSymbol is not IMethodSymbol currentMethod)
            return increment;

        foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
            if (symbolInfo.Symbol is IMethodSymbol invokedMethod)
            {
                // Check if there is a path from the invoked method back to the current method.
                visited.Clear();
                if (IsPathTo(invokedMethod, currentMethod, visited))
                {
                    increment++;
                    RaiseIncrementDiagnostic(context, GetKeywordLocation(invocation, invocation.SpanStart), "RecursionCycle", 0);
                }
            }
        }
        return increment;
    }

    private bool IsPathTo(IMethodSymbol from, IMethodSymbol target, HashSet<IMethodSymbol> visited)
    {
        if (from.Equals(target))
            return true;

        if (!visited.Add(from))
            return false;

        if (!GlobalMethodInvocationGraph.TryGetValue(from, out var invokedMethods))
            return false;

        foreach (var invokedMethod in invokedMethods)
        {
            if (IsPathTo(invokedMethod, target, visited))
                return true;
        }

        return false;
    }

    private void BuildGlobalMethodInvocationGraph(CompilationStartAnalysisContext context)
    {
        GlobalMethodInvocationGraph.Clear();

        foreach (var tree in context.Compilation.SyntaxTrees)
        {
            var root = tree.GetRoot(context.CancellationToken);
            var semanticModel = context.Compilation.GetSemanticModel(tree);

            foreach (var methodDeclaration in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                if (methodDeclaration.Body is null ||
                    methodDeclaration.Body.Statements.Count == 0)
                    continue;

                var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken) as IMethodSymbol;
                if (methodSymbol == null)
                    continue;

                if (!GlobalMethodInvocationGraph.TryGetValue(methodSymbol, out var invokedMethods))
                {
                    invokedMethods = new List<IMethodSymbol>();
                    GlobalMethodInvocationGraph[methodSymbol] = invokedMethods;
                }

                foreach (var invocation in methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    var invokedSymbol = semanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol;
                    if (invokedSymbol != null)
                    {
                        invokedMethods.Add(invokedSymbol);
                    }
                }
            }
        }
    }

    #endregion

    private void LoadCognitiveComplexityThreshold(Compilation compilation)
    {
        string? directoryPath = compilation.FileSystem?.GetDirectoryPath();
        LinterSettings.Create(directoryPath);
        this.complexityThreshold = LinterSettings.instance?.cognitiveComplexityThreshold ?? 15;
    }

    private void RaiseIncrementDiagnostic(CodeBlockAnalysisContext context, Location location, string category, int nestingPenalty)
    {
        context.ReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptors.Rule0089IncrementCognitiveComplexity,
                location,
                category,
                nestingPenalty + 1,
                nestingPenalty));
    }

    private Location GetKeywordLocation(SyntaxNode node, int spanStart)
    {
        return node switch
        {
            IfStatementSyntax ifStatement =>
                ifStatement.IfKeywordToken.GetLocation(),

            CaseStatementSyntax caseStatement =>
                caseStatement.CaseKeywordToken.GetLocation(),

            ForStatementSyntax forStatement =>
                forStatement.ForKeywordToken.GetLocation(),

            ForEachStatementSyntax forEachStatement =>
                forEachStatement.ForEachKeywordToken.GetLocation(),

            WhileStatementSyntax whileStatement =>
                whileStatement.WhileKeywordToken.GetLocation(),

            RepeatStatementSyntax repeatStatement =>
                repeatStatement.RepeatKeywordToken.GetLocation(),

#if !LessThenFall2024
            ConditionalExpressionSyntax conditionalExpression =>
                conditionalExpression.QuestionToken.GetLocation(),
#endif

            BinaryExpressionSyntax binaryExpression when
                node.IsKind(SyntaxKind.LogicalAndExpression) ||
                node.IsKind(SyntaxKind.LogicalOrExpression) ||
                node.IsKind(SyntaxKind.LogicalXorExpression)
                => binaryExpression.OperatorToken.GetLocation(),

            InvocationExpressionSyntax invocationExpression =>
                invocationExpression.Expression switch
                {
                    IdentifierNameSyntax identifier => identifier.Identifier.GetLocation(),
                    MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.GetLocation(),
                    _ => invocationExpression.GetLocation()
                },

            _ => Location.Create(node.GetLocation().SourceTree!, new TextSpan(spanStart, 1))
        };
    }
}