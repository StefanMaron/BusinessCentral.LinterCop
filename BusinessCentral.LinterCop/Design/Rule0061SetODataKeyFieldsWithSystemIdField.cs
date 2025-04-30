using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0061SetODataKeyFieldsWithSystemIdField : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0061SetODataKeyFieldsWithSystemIdField);

    public override void Initialize(AnalysisContext context)
        => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeODataKeyFieldsPropertyOnApiPage), SymbolKind.Page);

    private void AnalyzeODataKeyFieldsPropertyOnApiPage(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IPageTypeSymbol pageTypeSymbol)
            return;

        if (pageTypeSymbol.PageType != PageTypeKind.API)
            return;

        if (pageTypeSymbol.GetBooleanPropertyValue(PropertyKind.SourceTableTemporary).GetValueOrDefault())
            return;

        if (pageTypeSymbol.GetProperty(PropertyKind.ODataKeyFields) is not IPropertySymbol property)
            return;

        // Set the location of the diagnostic on the property itself (if exists)
        Location location = pageTypeSymbol.GetLocation();
        if (property is not null)
            location = property.GetLocation();

        if (property is null || property.Value is null || property.ValueText != "2000000000")
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0061SetODataKeyFieldsWithSystemIdField,
                location));
    }
}