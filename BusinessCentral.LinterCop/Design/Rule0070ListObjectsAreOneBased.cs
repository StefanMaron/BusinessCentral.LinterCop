using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0070ListObjectsAreOneBased : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0070ListObjectsAreOneBased);

    public override void Initialize(AnalysisContext context)
     => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.Analyze), OperationKind.InvocationExpression);

    private void Analyze(OperationAnalysisContext context)
    {
        if (context.IsObsoletePendingOrRemoved())
            return;
        if (IsExitCondition(context))
            return;

        switch (((IInvocationExpression)context.Operation).TargetMethod.Name)
        {
            case "Get":
                AnalyzeGetOperator(context);
                break;
            case "Count":
                AnalyzeCountOperator(context);
                break;
            default:
                break;
        }
    }

    private static void AnalyzeGetOperator(OperationAnalysisContext context)
    {
        if (context.Operation is not IInvocationExpression operation)
            return;
        if (operation.Arguments.Length < 1)
            return;

        switch (operation.Arguments[0].Syntax)
        {
            case LiteralExpressionSyntax literalExpressionSyntax:
                if (literalExpressionSyntax.Literal.GetLiteralValue().ToString() == "0")
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.Rule0070ListObjectsAreOneBased,
                        operation.Syntax.GetLocation()));
                }
                break;

            default:
                break;
        }
    }

    private static void AnalyzeCountOperator(OperationAnalysisContext context)
    {
        if (context.Operation is not IInvocationExpression operation)
            return;
        if (operation.Syntax.Parent is not ForStatementSyntax statementSyntax)
            return;
        if (statementSyntax.InitialValue is not LiteralExpressionSyntax expressionSyntax)
            return;
        if (expressionSyntax.Literal is not Int32SignedLiteralValueSyntax valueSyntax)
            return;

        if (valueSyntax.Number.ValueText == "0")
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0070ListObjectsAreOneBased,
                valueSyntax.GetLocation()));
        }
    }

    private static bool IsExitCondition(OperationAnalysisContext ctx)
    {
        if (ctx.Operation is not IInvocationExpression operation)
            return true;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod)
            return true;

        if (operation.TargetMethod.ContainingType?.GetNavTypeKindSafe() != NavTypeKind.List)
            return true;

        return false;
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0070ListObjectsAreOneBased = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0070",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0070ListObjectsAreOneBasedTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0070ListObjectsAreOneBasedFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0070ListObjectsAreOneBasedDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0070");
    }
}