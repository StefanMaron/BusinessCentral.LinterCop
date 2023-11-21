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
                if (IsTableOfTypeSetupTable(table)) return;

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

        private static bool IsTableOfTypeSetupTable(ITableTypeSymbol table)
        {
            // Expect Primary Key to contains only one field
            if (table.PrimaryKey is null || table.PrimaryKey.Fields.Length != 1) return false;

            // The field should be of type Code
            if (table.PrimaryKey.Fields[0].GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Code) return false;

            // The field should be exactly (case sensitive) called 'Primary Key'
            if (table.PrimaryKey.Fields[0].Name != "Primary Key") return false;

            return (true);
        }
    }
}
