using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0004LookupPageIdAndDrillDownPageId : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForLookupPageIdAndDrillDownPageId), SymbolKind.Page);

        private void CheckForLookupPageIdAndDrillDownPageId(SymbolAnalysisContext context)
        {
            if (context.Symbol.IsObsoletePending || context.Symbol.IsObsoleteRemoved) return;
            IPageTypeSymbol pageTypeSymbol = (IPageTypeSymbol)context.Symbol;
            if (pageTypeSymbol.PageType != PageTypeKind.List || pageTypeSymbol.RelatedTable == null) return;
            if (pageTypeSymbol.RelatedTable.ContainingModule != context.Symbol.ContainingModule) return;
            CheckTable(pageTypeSymbol.RelatedTable, ref context);
        }

        private void CheckTable(ITableTypeSymbol table, ref SymbolAnalysisContext context)
        {
            if (table.IsObsoletePending || table.IsObsoleteRemoved) return;
            if (!IsSymbolAccessible(table)) return;
            if (table.TableType == TableTypeKind.Temporary) return;

            bool exists = table.Properties.Where(e => e.PropertyKind == PropertyKind.DrillDownPageId || e.PropertyKind == PropertyKind.LookupPageId).Count() == 2;
            if (exists) return;

            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId,
                    table.GetLocation(),
                    new object[] { GetDeclaration(table), table.Name, context.Symbol.Name }));
        }

        private static string GetDeclaration(ISymbol symbol)
            => symbol.Location.SourceTree.GetText(CancellationToken.None).GetSubText(symbol.DeclaringSyntaxReference.Span).ToString();

        private static bool IsSymbolAccessible(ISymbol symbol)
        {
            try
            {
                GetDeclaration(symbol);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}
