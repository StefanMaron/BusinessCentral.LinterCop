using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0021ConfirmImplementConfirmManagement : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckConfirm), OperationKind.InvocationExpression);

        private void CheckConfirm(OperationAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;

            if (operation.TargetMethod.Name.ToUpper() == "CONFIRM" && operation.TargetMethod.MethodKind == MethodKind.BuiltInMethod)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement, ctx.Operation.Syntax.GetLocation()));
        }
    }
}
