#nullable disable // TODO: Enable nullable and review rule
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0031RecordInstanceIsolationLevel : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0031RecordInstanceIsolationLevel);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckLockTable), OperationKind.InvocationExpression);

        private void CheckLockTable(OperationAnalysisContext ctx)
        {
            if (!VersionChecker.IsSupported(ctx.ContainingSymbol, VersionCompatibility.Spring2023OrGreater)) return;
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "LockTable")) return;

            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0031RecordInstanceIsolationLevel, ctx.Operation.Syntax.GetLocation()));
        }
    }
}