using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForSingleFieldPrimaryKeysNotBlank), SymbolKind.Field);
        }
        private void CheckForSingleFieldPrimaryKeysNotBlank(SymbolAnalysisContext context)
        {
            if (context.IsObsoletePendingOrRemoved()) return;

            IFieldSymbol field = (IFieldSymbol)context.Symbol;
            if (GetExitCondition(field))
                return;

            ITableTypeSymbol table = (ITableTypeSymbol)field.GetContainingObjectTypeSymbol();
            if (table.PrimaryKey.Fields.Length != 1)
                return;

            if (table.PrimaryKey.Fields[0].Equals(field))
                if (field.GetProperty(PropertyKind.NotBlank) == null)
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys, field.GetLocation()));
        }

        private static bool GetExitCondition(IFieldSymbol field)
        {
            return
                field.FieldClass != FieldClassKind.Normal ||
                field.GetContainingObjectTypeSymbol().Kind != SymbolKind.Table ||
                !field.DeclaringSyntaxReference.GetSyntax().DescendantNodes().Any(Token => Token.Kind == SyntaxKind.LengthDataType) ||
                TableContainsNoSeries((ITableTypeSymbol)field.GetContainingObjectTypeSymbol());
        }

        private static bool TableContainsNoSeries(ITableTypeSymbol table)
        {
            return table.Fields.Any(field =>
            {
                var property = field.GetProperty(PropertyKind.TableRelation);
                if (property != null)
                    return field.GetProperty(PropertyKind.TableRelation).ValueText.UnquoteIdentifier().StartsWith("No. Series");
                return false;
            });
        }
    }
}
