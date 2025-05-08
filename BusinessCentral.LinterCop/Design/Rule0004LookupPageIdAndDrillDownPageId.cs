using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0004LookupPageIdAndDrillDownPageId : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForLookupPageIdAndDrillDownPageId), SymbolKind.Page);

    private void CheckForLookupPageIdAndDrillDownPageId(SymbolAnalysisContext context)
    {
        if (context.IsObsoletePendingOrRemoved() || context.Symbol is not IPageTypeSymbol pageTypeSymbol)
            return;

        if (pageTypeSymbol.PageType != PageTypeKind.List ||
            pageTypeSymbol.RelatedTable is null ||
            pageTypeSymbol.GetBooleanPropertyValue(PropertyKind.SourceTableTemporary).GetValueOrDefault() ||
            pageTypeSymbol.RelatedTable.ContainingModule != context.Symbol.ContainingModule)
            return;

        AnalyzeRelatedTable(pageTypeSymbol.RelatedTable, context);
    }

    private void AnalyzeRelatedTable(ITableTypeSymbol table, SymbolAnalysisContext context)
    {
        if (table.TableType == TableTypeKind.Temporary ||
            !table.GetLocation().IsInSource ||
            table.IsObsoletePendingOrRemoved())
            return;

        bool hasRequiredProperties = table.Properties.Count(property =>
            property.PropertyKind is PropertyKind.DrillDownPageId or PropertyKind.LookupPageId) == 2;

        if (hasRequiredProperties)
            return;

        context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId,
                table.GetLocation(),
                table.Name.ToString().QuoteIdentifierIfNeeded(),
                context.Symbol.Name.ToString().QuoteIdentifierIfNeeded()));
    }
}