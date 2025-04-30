using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0007DataPerCompanyShouldAlwaysBeSet : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0007DataPerCompanyShouldAlwaysBeSet);

    public override void Initialize(AnalysisContext context)
        => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForMissingDataPerCompanyOnTables), SymbolKind.Table);

    private void CheckForMissingDataPerCompanyOnTables(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not ITableTypeSymbol table)
            return;

        if (table.TableType == TableTypeKind.Temporary)
            return;

        if (table.GetProperty(PropertyKind.DataPerCompany) is null)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0007DataPerCompanyShouldAlwaysBeSet,
                table.GetLocation()));
        }
    }
}