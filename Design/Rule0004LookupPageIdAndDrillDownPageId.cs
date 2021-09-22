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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForLookupPageIdAndDrilldownPageId), new SymbolKind[]
            {
                SymbolKind.Page,
                SymbolKind.Table,
                SymbolKind.Property
            });
        }

        private void CheckForLookupPageIdAndDrilldownPageId(SymbolAnalysisContext context)
        {
            ISymbol symbol = context.Symbol;

            switch (symbol.Kind) {
                case SymbolKind.Page:
                    IPageTypeSymbol pageTypeSymbol = (IPageTypeSymbol)symbol;
                    if (pageTypeSymbol.PageType == PageTypeKind.List && pageTypeSymbol.RelatedTable != null)
                        CheckTable(pageTypeSymbol.RelatedTable, ref context);

                    break;

                default:
                    return;
            }
        }

        private void CheckTable(ITableTypeSymbol table, ref SymbolAnalysisContext context) {
            foreach (IPropertySymbol property in table.Properties) {
                if (property.PropertyKind == PropertyKind.DrillDownPageId ||
                    property.PropertyKind == PropertyKind.LookupPageId) {
                    return;
                }
            }

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
