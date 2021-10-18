using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0004LookupPageIdAndDrillDownPageId : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForLookupPageIdAndDrilldownPageId), SymbolKind.Page);

        private void CheckForLookupPageIdAndDrilldownPageId(SymbolAnalysisContext context)
        {
            if (context.Symbol.IsObsoletePending || context.Symbol.IsObsoleteRemoved) return;
            IPageTypeSymbol pageTypeSymbol = (IPageTypeSymbol)context.Symbol;
            if (pageTypeSymbol.PageType == PageTypeKind.List && pageTypeSymbol.RelatedTable != null)
                CheckTable(pageTypeSymbol.RelatedTable, ref context);
        }

        private void CheckTable(ITableTypeSymbol table, ref SymbolAnalysisContext context) {
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

        private static bool IsSymbolAccessible(ISymbol symbol) {
            try {
                GetDeclaration(symbol);
                return true;
            } catch(Exception) {
                return false;
            }
        }
    }
    
}
