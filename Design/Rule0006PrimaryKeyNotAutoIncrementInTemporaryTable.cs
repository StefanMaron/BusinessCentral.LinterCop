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
    public class Rule0006PrimaryKeyNotAutoIncrementInTemporaryTable : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0006PrimaryKeyNotAutoIncrementInTemporaryTable);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckTablePrimaryKeyIsNotAutoIncrement), SymbolKind.Table);

        private void CheckTablePrimaryKeyIsNotAutoIncrement(SymbolAnalysisContext context)
        {
            ITableTypeSymbol tableTypeSymbol = (ITableTypeSymbol)context.Symbol;
            if (!IsSymbolAccessible(tableTypeSymbol))
                return;

            CheckTable(tableTypeSymbol, ref context);
        }

        private void CheckTable(ITableTypeSymbol table, ref SymbolAnalysisContext context) {
            if (table.TableType != TableTypeKind.Temporary)
                return;

            foreach (var field in table.PrimaryKey.Fields) {
                IPropertySymbol propertySymbol = field.GetProperty(PropertyKind.AutoIncrement);
                if (propertySymbol != null) {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.Rule0006PrimaryKeyNotAutoIncrementInTemporaryTable,
                            propertySymbol.GetLocation(),
                            new object[] { GetDeclaration(propertySymbol), field.Name }));
                }
            }
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
