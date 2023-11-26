using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0034ExtensiblePropertyShouldAlwaysBeSet : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0034ExtensiblePropertyShouldAlwaysBeSet);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForMissingExtensibleProperty), new SymbolKind[] {
                SymbolKind.Table,
                SymbolKind.Page,
                SymbolKind.Report
            });

        private void CheckForMissingExtensibleProperty(SymbolAnalysisContext ctx)
        {
            if (ctx.Symbol.IsObsoletePending || ctx.Symbol.IsObsoleteRemoved) return;

            if (ctx.Symbol.GetTypeSymbol().Kind == SymbolKind.Table && ctx.Symbol.DeclaredAccessibility != Accessibility.Public) return;
            if (ctx.Symbol.GetTypeSymbol().Kind == SymbolKind.Page && ((IPageTypeSymbol)ctx.Symbol.GetTypeSymbol()).PageType == PageTypeKind.API) return;

            if (ctx.Symbol.GetProperty(PropertyKind.Extensible) != null) return;

            // The Extensible property (and DeclaredAccessibility) is supported from runtime versions 4.0 or greater.
            var manifest = AppSourceCopConfigurationProvider.GetManifest(ctx.Compilation);
            if (manifest.Runtime < RuntimeVersion.Fall2019) return;

            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0034ExtensiblePropertyShouldAlwaysBeSet, ctx.Symbol.GetLocation(), new object[] { Accessibility.Public.ToString().ToLower() }));
        }
    }
}