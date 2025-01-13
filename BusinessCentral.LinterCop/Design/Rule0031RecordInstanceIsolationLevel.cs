using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0031RecordInstanceIsolationLevel : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0031RecordInstanceIsolationLevel);

    public override VersionCompatibility SupportedVersions => VersionCompatibility.Spring2023OrGreater;

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckLockTable), OperationKind.InvocationExpression);

    private void CheckLockTable(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.TargetMethod.Name != "LockTable")
            return;

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0031RecordInstanceIsolationLevel,
            ctx.Operation.Syntax.GetLocation()));
    }
}