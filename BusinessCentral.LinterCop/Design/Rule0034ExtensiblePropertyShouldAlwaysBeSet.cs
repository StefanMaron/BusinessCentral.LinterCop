using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0034ExtensiblePropertyShouldAlwaysBeSet : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0034ExtensiblePropertyShouldAlwaysBeSet);

    public override VersionCompatibility SupportedVersions => VersionCompatibility.Fall2019OrGreater;

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForMissingExtensibleProperty), new SymbolKind[] {
                SymbolKind.Table,
                SymbolKind.Page,
                SymbolKind.Report
        });

    private void CheckForMissingExtensibleProperty(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        var typeSymbol = ctx.Symbol.GetTypeSymbol();

        if (typeSymbol.Kind == SymbolKind.Table && ctx.Symbol.DeclaredAccessibility != Accessibility.Public)
            return;

        if (typeSymbol.Kind == SymbolKind.Page && ((IPageTypeSymbol)ctx.Symbol.GetTypeSymbol()).PageType == PageTypeKind.API)
            return;

        if (ctx.Symbol.GetProperty(PropertyKind.Extensible) is null)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0034ExtensiblePropertyShouldAlwaysBeSet,
                ctx.Symbol.GetLocation(),
                Accessibility.Public.ToString().ToLower()));
    }
}