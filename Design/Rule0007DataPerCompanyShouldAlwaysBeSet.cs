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
    public class Rule0007DataPerCompanyShouldAlwaysBeSet : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0007DataPerCompanyShouldAlwaysBeSet);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckforMissingDataPerCompanyOnTables), SymbolKind.Table);

        private void CheckforMissingDataPerCompanyOnTables(SymbolAnalysisContext context)
        {
            if (context.Symbol.IsObsoletePending ||context.Symbol.IsObsoleteRemoved) return;
            ITableTypeSymbol table = (ITableTypeSymbol)context.Symbol;
            if (table.TableType == TableTypeKind.Temporary)
                return;

            if (!IsSymbolAccessible(table))
                return;

            if (table.GetProperty(PropertyKind.DataPerCompany) == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0007DataPerCompanyShouldAlwaysBeSet, table.GetLocation()));
            }
        }

        private static bool IsSymbolAccessible(ISymbol symbol) {
            try {
                GetDeclaration(symbol);
                return true;
            } catch(Exception) {
                return false;
            }
        }

        private static string GetDeclaration(ISymbol symbol)
            => symbol.Location.SourceTree.GetText(CancellationToken.None).GetSubText(symbol.DeclaringSyntaxReference.Span).ToString();
    }
    
}
