using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0023AlwaysSpecifyFieldgroups : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups, DiagnosticDescriptors.Rule0000ErrorInRule);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckFieldgroups), SymbolKind.Table);

        private void CheckFieldgroups(SymbolAnalysisContext ctx)
        {
            if (ctx.Symbol.IsObsoletePending || ctx.Symbol.IsObsoleteRemoved) return;
            try
            {
                ITableTypeSymbol table = (ITableTypeSymbol)ctx.Symbol;
                if (table.Length > 0)
                {
                    if (IsTableOfTypeSetupTable(table.Fields[0])) return;
                }

                Location FieldGroupLocation = table.GetLocation();
                if (!table.Keys.IsEmpty)
                {
                    FieldGroupLocation = table.Keys.Last().GetLocation();
                    var span = FieldGroupLocation.SourceSpan;
                    FieldGroupLocation = Location.Create(FieldGroupLocation.SourceTree, new TextSpan(span.End + 9, 1)); //Should result in the blank line right after the keys section
                }

                if (!table.FieldGroups.Any(item => (item.Name == "Brick")))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups, FieldGroupLocation, "Brick", table.Name));

                if (!table.FieldGroups.Any(item => (item.Name == "DropDown")))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups, FieldGroupLocation, "DropDown", table.Name));
            }
            catch (ArgumentOutOfRangeException)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0000ErrorInRule, ctx.Symbol.GetLocation(), new Object[] { "Rule0023", "ArgumentOutOfRangeException", "" }));
            }
        }

        private static bool IsTableOfTypeSetupTable(IFieldSymbol field)
        {
            // The first field of the table should be of type Code and exactly (case sensitive) called 'Primary Key'
            return (field.GetTypeSymbol().NavTypeKind == NavTypeKind.Code && field.Name == "Primary Key");
        }
    }
}
