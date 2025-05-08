using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0019DataClassificationFieldEqualsTable : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0019DataClassificationFieldEqualsTable);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(CheckDataClassificationRedundancy), SymbolKind.Field);

    private void CheckDataClassificationRedundancy(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IFieldSymbol field)
            return;

        IApplicationObjectTypeSymbol? applicationObject = field.GetContainingApplicationObjectTypeSymbol();
        if (applicationObject is not ITableTypeSymbol || applicationObject.IsObsoletePendingOrRemoved() || field.ContainingSymbol is not ITableTypeSymbol table)
            return;

        IPropertySymbol? fieldClassification = field.GetProperty(PropertyKind.DataClassification);
        if (fieldClassification is null)
            return;

        IPropertySymbol? tableClassification = table.GetProperty(PropertyKind.DataClassification);
        if (tableClassification is null)
            return;

        if (fieldClassification.ValueText == tableClassification.ValueText)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0019DataClassificationFieldEqualsTable,
                fieldClassification.GetLocation()));
    }
}