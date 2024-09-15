#if Fall2023RV1
using BusinessCentral.LinterCop.AnalysisContextExtension;
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
            if (!VersionChecker.IsSupported(ctx.Symbol, Feature.AddPageControlInPageCustomization)) return;
            if (ctx.IsObsoletePendingOrRemoved()) return;

            ICollection<IFieldSymbol> tableFields = GetTableFields(ctx.Symbol).Where(x => x.Id > 0 && x.Id < 2000000000)
                                                                .Where(x => x.DeclaredAccessibility != Accessibility.Local && x.DeclaredAccessibility != Accessibility.Protected)
                                                                .Where(x => x.FieldClass != FieldClassKind.FlowFilter)
                                                                .Where(x => x.GetBooleanPropertyValue(PropertyKind.Enabled) != false)
                                                                .Where(x => x.GetProperty(PropertyKind.AllowInCustomizations) is null)
                                                                .Where(x => x.GetProperty(PropertyKind.ObsoleteState) is null)
                                                                .Where(x => IsSupportedType(x.OriginalDefinition.GetTypeSymbol().GetNavTypeKindSafe()))
                                                                .ToList();
            if (!tableFields.Any()) return;

            IEnumerable<IApplicationObjectTypeSymbol>? relatedPages = GetRelatedPages(ctx);

            if (!relatedPages.Any())
            {
                if (ctx.Symbol.GetTypeSymbol().Kind != SymbolKind.TableExtension)
                    return;
                ITableExtensionTypeSymbol tableExtension = (ITableExtensionTypeSymbol)ctx.Symbol;
                if (tableExtension.Target is not null && !LookupOrDrillDownPageIsSet((ITableTypeSymbol)tableExtension.Target))
                    return;
                // allows diagnostic for table extension fields where base table has lookup or drilldown page set
                // even if no relatedPages exist directly
            }

            ICollection<IFieldSymbol> pageFields = GetPageFields(relatedPages);
            ICollection<IFieldSymbol> fieldsNotReferencedOnPage = tableFields.Except(pageFields).ToList();
            foreach (IFieldSymbol field in fieldsNotReferencedOnPage)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0035ExplicitSetAllowInCustomizations, field.Location!));
        }

        private static ICollection<IFieldSymbol> GetTableFields(ISymbol symbol)
        {
            switch (symbol.GetContainingObjectTypeSymbol().GetNavTypeKindSafe())
            {
                case NavTypeKind.Record:
                    return ((ITableTypeSymbol)symbol).Fields;
                case NavTypeKind.TableExtension:
                    return ((ITableExtensionTypeSymbol)symbol).AddedFields;
                default:
                    return new Collection<IFieldSymbol>();
            }
        }

        private static ICollection<IFieldSymbol> GetPageFields(IEnumerable<IApplicationObjectTypeSymbol>? relatedPages)
        {
            if (relatedPages == null)
                return [];

            ICollection<IFieldSymbol> pageFields = new Collection<IFieldSymbol>();
            foreach (IApplicationObjectTypeSymbol relatedPageLike in relatedPages)
            {
                switch (relatedPageLike.GetNavTypeKindSafe())
                {
                    case NavTypeKind.Page:
                        IEnumerable<IFieldSymbol> fields = ((IPageTypeSymbol)relatedPageLike).FlattenedControls.Where(x => x.ControlKind == ControlKind.Field && x.RelatedFieldSymbol != null)
                                                            .Select(x => (IFieldSymbol)x.RelatedFieldSymbol!.OriginalDefinition);
                        pageFields = pageFields.Union(fields).Distinct().ToList();
                        break;
                    case NavTypeKind.PageExtension:
                        IEnumerable<IFieldSymbol> extFields = ((IPageExtensionTypeSymbol)relatedPageLike).AddedControlsFlattened.Where(x => x.ControlKind == ControlKind.Field && x.RelatedFieldSymbol != null)
                                                            .Select(x => (IFieldSymbol)x.RelatedFieldSymbol!.OriginalDefinition);

                        pageFields = pageFields.Union(extFields).Distinct().ToList();
                        break;
                }
            }
            return pageFields;
        }

        private static IEnumerable<IApplicationObjectTypeSymbol>? GetRelatedPages(SymbolAnalysisContext ctx)
        {
            // table and tableextension fields can each be referenced on both pages and pageextensions
            ITableTypeSymbol? table = null;
            switch (ctx.Symbol.GetContainingObjectTypeSymbol().GetNavTypeKindSafe())
            {
                case NavTypeKind.Record:
                    table = ctx.Symbol as ITableTypeSymbol;
                    break;
                case NavTypeKind.TableExtension:
                    if (ctx.Symbol is IApplicationObjectExtensionTypeSymbol typeSymbol)
                        table = typeSymbol.Target as ITableTypeSymbol;
                    break;
                default:
                    return null;
            }

            if (table is null)
                return [];

            IEnumerable<IApplicationObjectTypeSymbol> pages = ctx.Compilation.GetDeclaredApplicationObjectSymbols()
                                            .Where(x => x.GetNavTypeKindSafe() == NavTypeKind.Page)
                                            .Where(x => ((IPageTypeSymbol)x.GetTypeSymbol()).PageType != PageTypeKind.API)
                                            .Where(x => ((IPageTypeSymbol)x.GetTypeSymbol()).RelatedTable == table);

            IEnumerable<IApplicationObjectTypeSymbol> pageExtensions = ctx.Compilation.GetDeclaredApplicationObjectSymbols()
                                            .Where(x => x.GetNavTypeKindSafe() == NavTypeKind.PageExtension)
                                            .Where(x => ((IApplicationObjectExtensionTypeSymbol)x).Target != null)
                                            .Where(x => ((IPageTypeSymbol)((IApplicationObjectExtensionTypeSymbol)x).Target!.GetTypeSymbol()).RelatedTable == table);

            return pages.Union(pageExtensions);
        }

        private static bool IsSupportedType(NavTypeKind navTypeKind)
        {
            switch (navTypeKind)
            {
                case NavTypeKind.Blob:
                case NavTypeKind.Media:
                case NavTypeKind.MediaSet:
                case NavTypeKind.RecordId:
                case NavTypeKind.TableFilter:
                    return false;

                default:
                    return true;
            }
        }

        private static bool LookupOrDrillDownPageIsSet(ITableTypeSymbol table)
        {
            return table.Properties.Any(e => e.PropertyKind == PropertyKind.DrillDownPageId || e.PropertyKind == PropertyKind.LookupPageId);
        }
    }
}
#endif