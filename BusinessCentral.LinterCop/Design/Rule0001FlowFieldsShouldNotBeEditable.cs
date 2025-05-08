using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0001FlowFieldsShouldNotBeEditable : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeFlowFieldEditable), SymbolKind.Field);

    private void AnalyzeFlowFieldEditable(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IFieldSymbol field)
            return;

        if (field.FieldClass == FieldClassKind.FlowField &&
            field.GetBooleanPropertyValue(PropertyKind.Editable).GetValueOrDefault())
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable,
                field.GetLocation()));
        }
    }
}
