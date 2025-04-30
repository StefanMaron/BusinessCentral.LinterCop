using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0009CodeMetrics : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0009CodeMetricsInfo, DiagnosticDescriptors.Rule0010CodeMetricsWarning);

    private static readonly HashSet<string> eventPublisherDecoratorNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "BusinessEvent",
        "IntegrationEvent",
        "ExternalBusinessEvent"
    };

    private static readonly HashSet<SyntaxKind> OperatorAndOperandKinds =
        Enum.GetValues(typeof(SyntaxKind))
            .Cast<SyntaxKind>()
            .Where(value => value.ToString().Contains("Keyword") ||
                            value.ToString().Contains("Token") ||
                            IsOperandKind(value))
            .ToHashSet();

    public override void Initialize(AnalysisContext context) =>
        context.RegisterCodeBlockAction(new Action<CodeBlockAnalysisContext>(this.CheckCodeMetrics));

    private void CheckCodeMetrics(CodeBlockAnalysisContext context)
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

        var descendants = methodOrTrigger.Body.DescendantNodesAndTokens(e => true).ToArray();

        int cyclomaticComplexity = GetCyclomaticComplexity(descendants);
        double HalsteadVolume = GetHalsteadVolume(context, methodOrTrigger.Body, descendants, cyclomaticComplexity);

        if (LinterSettings.instance is null)
            LinterSettings.Create(context.SemanticModel.Compilation.FileSystem?.GetDirectoryPath());
        LinterSettings settings = LinterSettings.instance!;

        if (cyclomaticComplexity >= settings.cyclomaticComplexityThreshold || Math.Round(HalsteadVolume) <= settings.maintainabilityIndexThreshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0010CodeMetricsWarning, context.OwningSymbol.GetLocation(), new object[] { cyclomaticComplexity, settings.cyclomaticComplexityThreshold, Math.Round(HalsteadVolume), settings.maintainabilityIndexThreshold }));
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0009CodeMetricsInfo, context.OwningSymbol.GetLocation(), new object[] { cyclomaticComplexity, settings.cyclomaticComplexityThreshold, Math.Round(HalsteadVolume), settings.maintainabilityIndexThreshold }));
    }

    private static double GetHalsteadVolume(CodeBlockAnalysisContext context, SyntaxNode methodBodyNode,
        SyntaxNodeOrToken[] descendantNodesAndTokens, int cyclomaticComplexity)
    {
        try
        {
            var triviaLinesCount = methodBodyNode
                .DescendantTrivia(e => true, true)
                .Count(node =>
                    node.Kind == SyntaxKind.EndOfLineTrivia &&
                    node.GetLocation().GetLineSpan().StartLinePosition.Line ==
                    node.Token.GetLocation().GetLineSpan().StartLinePosition.Line) - 2; //Minus 2 for Begin end of function

            context.CancellationToken.ThrowIfCancellationRequested();
            var N = 0;
            using var hashSet = PooledHashSet<SyntaxNodeOrToken>.GetInstance();
            foreach (var nodeOrToken in descendantNodesAndTokens)
            {
                if (OperatorAndOperandKinds.Contains(nodeOrToken.Kind))
                {
                    N++;
                    hashSet.Add(nodeOrToken);
                }
            }

            double HalsteadVolume = N * Math.Log(hashSet.Count, 2);

            //171−5.2lnV−0.23G−16.2lnL
            return Math.Max(0, (171 - 5.2 * Math.Log(HalsteadVolume) - 0.23 * cyclomaticComplexity - 16.2 * Math.Log(triviaLinesCount)) * 100 / 171);
        }
        catch (System.NullReferenceException)
        {
            return 0.0;
        }
    }

    private static int GetCyclomaticComplexity(SyntaxNodeOrToken[] nodesAndTokens)
    {
        return nodesAndTokens.Count(syntaxNodeOrToken => IsComplexKind(syntaxNodeOrToken.Kind)) + 1;
    }

    private static bool IsOperandKind(SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.IdentifierToken:
            case SyntaxKind.Int32LiteralToken:
            case SyntaxKind.StringLiteralToken:
            case SyntaxKind.BooleanLiteralValue:
            case SyntaxKind.TrueKeyword:
            case SyntaxKind.FalseKeyword:
                return true;
        }

        return false;
    }

    private static bool IsComplexKind(SyntaxKind kind)
    {
        switch (kind)
        {
            case SyntaxKind.IfKeyword:
            case SyntaxKind.ElifKeyword:
            case SyntaxKind.LogicalAndExpression:
            case SyntaxKind.LogicalOrExpression:
            case SyntaxKind.CaseLine:
            case SyntaxKind.ForKeyword:
            case SyntaxKind.ForEachKeyword:
            case SyntaxKind.WhileKeyword:
            case SyntaxKind.UntilKeyword:
#if !LessThenFall2024
            case SyntaxKind.ConditionalExpression: // Ternary operator
#endif
                return true;
        }

        return false;
    }
}