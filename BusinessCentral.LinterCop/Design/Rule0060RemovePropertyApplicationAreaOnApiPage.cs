using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0060PropertyApplicationAreaOnApiPage : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0060PropertyApplicationAreaOnApiPage);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzePropertyApplicationAreaOnApiPage), SymbolKind.Page);

    private void AnalyzePropertyApplicationAreaOnApiPage(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IPageTypeSymbol pageTypeSymbol)
            return;

        if (pageTypeSymbol.PageType != PageTypeKind.API)
            return;

        if (pageTypeSymbol.GetProperty(PropertyKind.ApplicationArea) is IPropertySymbol propertyApplicationArea)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0060PropertyApplicationAreaOnApiPage,
                propertyApplicationArea.GetLocation()));

        IEnumerable<Location> Locations = pageTypeSymbol.FlattenedControls
                                                        .Where(e => e.ControlKind == ControlKind.Field && e.GetProperty(PropertyKind.ApplicationArea) is not null)
                                                        .Select(e => e.GetLocation());

        foreach (Location location in Locations)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0060PropertyApplicationAreaOnApiPage,
                location));
    }
}