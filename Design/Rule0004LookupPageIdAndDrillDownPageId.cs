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
        {
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForLookupPageIdAndDrilldownPageId), new SymbolKind[]
            {
                SymbolKind.Page
            });
        }

        private void CheckForLookupPageIdAndDrilldownPageId(SymbolAnalysisContext context)
        {
            if (context.Symbol.Kind != SymbolKind.Page)
                return;

            IPageTypeSymbol pageTypeSymbol = (IPageTypeSymbol)context.Symbol;
            if (pageTypeSymbol.PageType == PageTypeKind.List && pageTypeSymbol.RelatedTable != null)
                CheckTable(pageTypeSymbol.RelatedTable, ref context);
        }

        private void CheckTable(ITableTypeSymbol table, ref SymbolAnalysisContext context) {
            bool exists = table.Properties.Where(e => e.PropertyKind == PropertyKind.DrillDownPageId || e.PropertyKind == PropertyKind.LookupPageId).Count() == 2;
            if (exists) return;

            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId,
                    context.Symbol.GetLocation(),
                    new object[] { GetDeclaration(table) }));
        }

        private static string GetDeclaration(ISymbol symbol)
            => symbol.Location.SourceTree.GetText(CancellationToken.None).GetSubText(symbol.DeclaringSyntaxReference.Span).ToString();
    }
    
}
