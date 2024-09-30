#nullable disable // TODO: Enable nullable and review rule
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0007DataPerCompanyShouldAlwaysBeSet : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0007DataPerCompanyShouldAlwaysBeSet);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForMissingDataPerCompanyOnTables), SymbolKind.Table);

        private void CheckForMissingDataPerCompanyOnTables(SymbolAnalysisContext context)
        {
            if (context.IsObsoletePendingOrRemoved()) return;
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

        private static string GetDeclaration(ISymbol symbol)
            => symbol.Location.SourceTree.GetText(CancellationToken.None).GetSubText(symbol.DeclaringSyntaxReference.Span).ToString();
    }
}