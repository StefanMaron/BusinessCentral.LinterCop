using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class BuiltInMethodImplementThroughCodeunit : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(
            DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement,
            DiagnosticDescriptors.Rule0022GlobalLanguageImplementTranslationHelper
        );

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeConfirm), OperationKind.InvocationExpression);
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeGlobalLanguage), OperationKind.InvocationExpression);
        }

        private void AnalyzeConfirm(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;
            if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "Confirm")) return;

            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().NavTypeKind == NavTypeKind.Page &&
            ((IPageTypeSymbol)ctx.ContainingSymbol.GetContainingObjectTypeSymbol()).PageType != PageTypeKind.API) return;

            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement, ctx.Operation.Syntax.GetLocation()));
        }

        private void AnalyzeGlobalLanguage(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;
            if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "GlobalLanguage")) return;
            if (operation.Arguments.Length == 0) return;

            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0022GlobalLanguageImplementTranslationHelper, ctx.Operation.Syntax.GetLocation()));
        }
    }
}