using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0035ExplicitSetAllowInCustomizations : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0035ExplicitSetAllowInCustomizations);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeAllowInCustomization), new SymbolKind[] {
                SymbolKind.Table,
                SymbolKind.TableExtension,
            });

        private void AnalyzeAllowInCustomization(SymbolAnalysisContext ctx)
        {
            if (ctx.Symbol.IsObsoletePending || ctx.Symbol.IsObsoleteRemoved) return;
            if (ctx.Symbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.Symbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;

            // The AllowInCustomizations property is supported from runtime versions 12.0 or greater.
            var manifest = AppSourceCopConfigurationProvider.GetManifest(ctx.Compilation);
            if (manifest.Runtime < RuntimeVersion.Fall2023) return;

            ICollection<IFieldSymbol> tableFields = GetTableFields(ctx.Symbol).Where(x => x.Id > 0 && x.Id < 2000000000)
                                                                .Where(x => x.GetProperty(PropertyKind.AllowInCustomizations) is null)
                                                                .ToList();
            if (!tableFields.Any()) return;

            IEnumerable<IApplicationObjectTypeSymbol> relatedPages = GetRelatedPages(ctx);
            if (!relatedPages.Any()) return;

            NavTypeKind navTypeKind = ctx.Symbol.GetContainingObjectTypeSymbol().GetNavTypeKindSafe();
            ICollection<IFieldSymbol> pageFields = GetPageFields(navTypeKind, relatedPages);
            ICollection<IFieldSymbol> fieldsNotReferencedOnPage = tableFields.Except(pageFields).ToList();
            foreach (IFieldSymbol field in fieldsNotReferencedOnPage)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0035ExplicitSetAllowInCustomizations, field.Location));
        }

        private static ICollection<IFieldSymbol> GetTableFields(ISymbol symbol)
        {
            switch (symbol.GetContainingObjectTypeSymbol().GetNavTypeKindSafe())
            {
                case NavTypeKind.Table:
                    return ((ITableTypeSymbol)symbol).Fields;
                case NavTypeKind.TableExtension:
                    return ((ITableExtensionTypeSymbol)symbol).AddedFields;
                default:
                    return new Collection<IFieldSymbol>();
            }
        }

        private static ICollection<IFieldSymbol> GetPageFields(NavTypeKind navTypeKind, IEnumerable<IApplicationObjectTypeSymbol> relatedPages)
        {
            ICollection<IFieldSymbol> pageFields = new Collection<IFieldSymbol>();
            switch (navTypeKind)
            {
                case NavTypeKind.Table:
                    foreach (IPageTypeSymbol page in relatedPages.Cast<IPageTypeSymbol>())
                    {
                        IEnumerable<IFieldSymbol> fields = page.FlattenedControls.Where(x => x.ControlKind == ControlKind.Field && x.RelatedFieldSymbol != null)
                                                                                    .Select(x => (IFieldSymbol)x.RelatedFieldSymbol.OriginalDefinition);

                        pageFields = pageFields.Union(fields).Distinct().ToList();
                    }
                    return pageFields;
                case NavTypeKind.TableExtension:
                    foreach (IPageExtensionTypeSymbol page in relatedPages.Cast<IPageExtensionTypeSymbol>())
                    {
                        IEnumerable<IFieldSymbol> fields = page.AddedControlsFlattened.Where(x => x.ControlKind == ControlKind.Field && x.RelatedFieldSymbol != null)
                                                                                    .Select(x => (IFieldSymbol)x.RelatedFieldSymbol.OriginalDefinition);

                        pageFields = pageFields.Union(fields).Distinct().ToList();
                    }
                    return pageFields;
                default:
                    return pageFields;
            }
        }

        private static IEnumerable<IApplicationObjectTypeSymbol> GetRelatedPages(SymbolAnalysisContext ctx)
        {
            switch (ctx.Symbol.GetContainingObjectTypeSymbol().GetNavTypeKindSafe())
            {
                case NavTypeKind.Table:
                    return ctx.Compilation.GetDeclaredApplicationObjectSymbols()
                                            .Where(x => x.GetNavTypeKindSafe() == NavTypeKind.Page)
                                            .Where(x => ((IPageTypeSymbol)x.GetTypeSymbol()).PageType != PageTypeKind.API)
                                            .Where(x => ((IPageTypeSymbol)x.GetTypeSymbol()).RelatedTable == (ITableTypeSymbol)ctx.Symbol);
                case NavTypeKind.TableExtension:
                    return ctx.Compilation.GetDeclaredApplicationObjectSymbols()
                                            .Where(x => x.GetNavTypeKindSafe() == NavTypeKind.PageExtension)
                                            .Where(x => ((IPageTypeSymbol)((IApplicationObjectExtensionTypeSymbol)x).Target.GetTypeSymbol()).RelatedTable == ((IApplicationObjectExtensionTypeSymbol)ctx.Symbol).Target);
                default:
                    return null;
            }
        }
    }
}