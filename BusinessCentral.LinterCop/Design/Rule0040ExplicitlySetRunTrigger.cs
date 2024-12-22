using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0040ExplicitlySetRunTrigger : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0040ExplicitlySetRunTrigger);

    private static readonly HashSet<string> buildInMethodNames = new()
        {
            "Insert",
            "Modify",
            "ModifyAll",
            "Delete",
            "DeleteAll"
        };

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeRunTriggerParameters), OperationKind.InvocationExpression);

    private void AnalyzeRunTriggerParameters(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            !buildInMethodNames.Contains(operation.TargetMethod.Name))
            return;

        var operationTypeSymbolNavType = operation.Instance?.GetSymbol()?.GetTypeSymbol().GetNavTypeKindSafe();
        if (operationTypeSymbolNavType is null ||
            !(operationTypeSymbolNavType == NavTypeKind.Record || operationTypeSymbolNavType == NavTypeKind.RecordRef))
            return;

        if (operation.Arguments.Where(args => args.Parameter.Name.Equals("RunTrigger")).SingleOrDefault() is null)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0040ExplicitlySetRunTrigger,
                ctx.Operation.Syntax.GetLocation()));
    }
}