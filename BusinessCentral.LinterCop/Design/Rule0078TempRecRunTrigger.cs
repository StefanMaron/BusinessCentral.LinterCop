using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0078TemporaryRecordsShouldNotTriggerTableTriggers : DiagnosticAnalyzer
{
    private static readonly HashSet<string> methodsToCheck = ["Insert", "Modify", "Delete", "DeleteAll", "Validate", "ModifyAll"];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create(DiagnosticDescriptors.Rule0078TemporaryRecordsShouldNotTriggerTableTriggers);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(AnalyzeTemporaryRecords, OperationKind.InvocationExpression);

    private void AnalyzeTemporaryRecords(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            !methodsToCheck.Contains(operation.TargetMethod.Name))
            return;

        if (operation.Instance?.Type is not IRecordTypeSymbol record ||
            !record.Temporary ||
            record.BaseTable.TableType == TableTypeKind.Temporary)
            return;

        bool isExecutingTriggersOrValidation = operation.TargetMethod.Name switch
        {
            "Validate" => true,
            "ModifyAll" => operation.Arguments.Length == 3 && IsRunTriggerEnabled(operation.Arguments[2]),
            _ => operation.Arguments.Length == 1 && IsRunTriggerEnabled(operation.Arguments[0])
        };

        if (isExecutingTriggersOrValidation)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0078TemporaryRecordsShouldNotTriggerTableTriggers,
                ctx.Operation.Syntax.GetLocation()));
    }

    private static bool IsRunTriggerEnabled(IArgument argument) =>
        argument.Value.ConstantValue.HasValue &&
        argument.Value.ConstantValue.Value is bool isEnabled &&
        isEnabled;
}