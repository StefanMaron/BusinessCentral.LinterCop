using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0078TemporaryRecordsShouldNotTriggerTableTriggers : DiagnosticAnalyzer
{
    private static readonly HashSet<string> methodsToCheck = new() { "Insert", "Modify", "Delete", "DeleteAll", "Validate", "ModifyAll" };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0078TemporaryRecordsShouldNotTriggerTableTriggers);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(AnalyzeTemporaryRecords, OperationKind.InvocationExpression);

    private void AnalyzeTemporaryRecords(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        var invocationExpression = (IInvocationExpression)ctx.Operation;
        if (!methodsToCheck.Contains(invocationExpression.TargetMethod.Name))
            return;

        if (!(invocationExpression.Instance?.Type is IRecordTypeSymbol record) || !record.Temporary)
            return;

        bool isExecutingTriggersOrValidation = invocationExpression.TargetMethod.Name switch
        {
            "Validate" => true,
            "ModifyAll" => invocationExpression.Arguments.Length == 3 &&
                          IsRunTriggerEnabled(invocationExpression.Arguments[2]),
            _ => invocationExpression.Arguments.Length == 1 &&
                 IsRunTriggerEnabled(invocationExpression.Arguments[0])
        };

        if (isExecutingTriggersOrValidation)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0078TemporaryRecordsShouldNotTriggerTableTriggers,
                ctx.Operation.Syntax.GetLocation()));
        }
    }

    private static bool IsRunTriggerEnabled(IArgument argument) =>
        argument.Value.ConstantValue.HasValue &&
        argument.Value.ConstantValue.Value is bool isEnabled &&
        isEnabled;

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0078TemporaryRecordsShouldNotTriggerTableTriggers = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0078",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0078TemporaryRecordsShouldNotTriggerTableTriggersTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0078TemporaryRecordsShouldNotTriggerTableTriggersFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0078TemporaryRecordsShouldNotTriggerTableTriggersDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0078");
    }
}
