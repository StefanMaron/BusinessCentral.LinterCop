using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class BuiltInMethodImplementThroughCodeunit : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement,
            DiagnosticDescriptors.Rule0022GlobalLanguageImplementTranslationHelper
        );

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), OperationKind.InvocationExpression);

    private void AnalyzeInvocation(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod)
            return;

        switch (operation.TargetMethod.Name)
        {
            case "Confirm":
                if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().NavTypeKind == NavTypeKind.Page &&
                ((IPageTypeSymbol)ctx.ContainingSymbol.GetContainingObjectTypeSymbol()).PageType != PageTypeKind.API)
                    return;

                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement,
                    ctx.Operation.Syntax.GetLocation()));
                break;

            case "GlobalLanguage":
                if (operation.Arguments.Length == 0)
                    return;

                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0022GlobalLanguageImplementTranslationHelper,
                    ctx.Operation.Syntax.GetLocation()));
                break;
        }
    }
}