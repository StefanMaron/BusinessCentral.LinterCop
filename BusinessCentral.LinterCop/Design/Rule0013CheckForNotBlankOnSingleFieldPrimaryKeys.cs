using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys, DiagnosticDescriptors.Rule0067DisableNotBlankOnSingleFieldPrimaryKey);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForSingleFieldPrimaryKeyNotBlankProperty), SymbolKind.Table);

    private void CheckForSingleFieldPrimaryKeyNotBlankProperty(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not ITableTypeSymbol table)
            return;

        if (table.PrimaryKey?.Fields == null || table.PrimaryKey.Fields.Length != 1)
            return;

        var field = table.PrimaryKey.Fields[0];
        if (!field.GetTypeSymbol().HasLength)
            return;

        if (TableContainsNoSeries(table))
        {
            if (field.GetBooleanPropertyValue(PropertyKind.NotBlank).GetValueOrDefault() && !SemanticFacts.IsSameName(field.Name, "Name"))
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0067DisableNotBlankOnSingleFieldPrimaryKey,
                    field.GetLocation()));
        }
        else
        {
            if (field.GetProperty(PropertyKind.NotBlank) is null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys,
                    field.GetLocation()));
            }
        }
    }

    private static bool TableContainsNoSeries(ITableTypeSymbol table)
    {
        return table.Fields
            .Where(x => x.FieldClass == FieldClassKind.Normal && x.Id > 0 && x.Id < 2000000000)
#if !LessThenFall2024
            .Where(x => x.Type?.GetNavTypeKindSafe() == NavTypeKind.Code)
#endif
            .Any(field =>
        {
            IPropertySymbol? propertySymbol = field.GetProperty(PropertyKind.TableRelation);
            if (propertySymbol is not null && propertySymbol.ContainingSymbol is not null)
                return SemanticFacts.IsSameName(propertySymbol.ContainingSymbol.Name.UnquoteIdentifier(), "No. Series");
            return false;
        });
    }
}