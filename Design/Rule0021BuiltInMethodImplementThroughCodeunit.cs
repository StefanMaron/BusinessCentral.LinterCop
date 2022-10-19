using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0021BuiltInMethodImplementThroughCodeunit : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement, DiagnosticDescriptors.Rule0022GlobalLanguageImplementTranslationHelper);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckConfirm), OperationKind.InvocationExpression);

        private void CheckConfirm(OperationAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            switch (operation.TargetMethod.Name.ToUpper())
            {
                case "CONFIRM":
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement, ctx.Operation.Syntax.GetLocation()));
                    break;
                case "GLOBALLANGUAGE":
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0022GlobalLanguageImplementTranslationHelper, ctx.Operation.Syntax.GetLocation()));
                    break;
            }
        }
    }
}
