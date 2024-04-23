using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0004LookupPageIdAndDrillDownPageId : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId, DiagnosticDescriptors.Rule0000ErrorInRule);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForLookupPageIdAndDrillDownPageId), SymbolKind.Page);

        private void CheckForLookupPageIdAndDrillDownPageId(SymbolAnalysisContext context)
        {
            if (context.IsObsoletePendingOrRemoved()) return;

            IPageTypeSymbol pageTypeSymbol = (IPageTypeSymbol)context.Symbol;
            if (pageTypeSymbol.PageType != PageTypeKind.List || pageTypeSymbol.RelatedTable == null) return;
            if (pageTypeSymbol.RelatedTable.ContainingModule != context.Symbol.ContainingModule) return;
            CheckTable(pageTypeSymbol.RelatedTable, context);
        }

        private void CheckTable(ITableTypeSymbol table, SymbolAnalysisContext context)
        {
            if (table.IsObsoletePendingOrRemoved()) return;

            if (!IsSymbolAccessible(table, context)) return;
            if (table.TableType == TableTypeKind.Temporary) return;

            bool exists = table.Properties.Where(e => e.PropertyKind == PropertyKind.DrillDownPageId || e.PropertyKind == PropertyKind.LookupPageId).Count() == 2;
            if (exists) return;

            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId,
                    table.GetLocation(),
                    new object[] { GetDeclaration(table, context), table.Name, context.Symbol.Name }));
        }

        private static string GetDeclaration(ISymbol symbol, SymbolAnalysisContext context)
            => symbol.Location.SourceTree.GetText(context.CancellationToken).GetSubText(symbol.DeclaringSyntaxReference.Span).ToString();

        private static bool IsSymbolAccessible(ISymbol symbol, SymbolAnalysisContext context)
        {
            try
            {
                GetDeclaration(symbol, context);
                return true;
            }
            catch (Exception)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0000ErrorInRule, context.Symbol.GetLocation(), new Object[] { "Rule0004", "Exception", "at Line 47" }));
                return false;
            }
        }
    }
}