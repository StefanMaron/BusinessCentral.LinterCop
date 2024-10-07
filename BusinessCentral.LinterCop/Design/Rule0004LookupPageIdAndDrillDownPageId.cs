#nullable disable // TODO: Enable nullable and review rule
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0004LookupPageIdAndDrillDownPageId : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId);

    public override void Initialize(AnalysisContext context)
        => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForLookupPageIdAndDrillDownPageId), SymbolKind.Page);

    private void CheckForLookupPageIdAndDrillDownPageId(SymbolAnalysisContext context)
    {
        if (context.IsObsoletePendingOrRemoved()) return;

        IPageTypeSymbol pageTypeSymbol = (IPageTypeSymbol)context.Symbol;
        if (pageTypeSymbol.PageType != PageTypeKind.List || pageTypeSymbol.RelatedTable == null) return;
        if (pageTypeSymbol.GetBooleanPropertyValue(PropertyKind.SourceTableTemporary).GetValueOrDefault()) return;
        if (pageTypeSymbol.RelatedTable.ContainingModule != context.Symbol.ContainingModule) return;
        CheckTable(pageTypeSymbol.RelatedTable, context);
    }

    private void CheckTable(ITableTypeSymbol table, SymbolAnalysisContext context)
    {
        if (table.IsObsoletePendingOrRemoved()) return;

        if (!table.GetLocation().IsInSource) return;
        if (table.TableType == TableTypeKind.Temporary) return;

        bool exists = table.Properties.Where(e => e.PropertyKind == PropertyKind.DrillDownPageId || e.PropertyKind == PropertyKind.LookupPageId).Count() == 2;
        if (exists) return;

        context.ReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId,
                table.GetLocation(),
                new object[] { table.Name.ToString().QuoteIdentifierIfNeeded(), context.Symbol.Name.ToString().QuoteIdentifierIfNeeded() }));
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0004LookupPageIdAndDrillDownPageId = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0004",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0004LookupPageIdAndDrillDownPageIdTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0004LookupPageIdAndDrillDownPageIdFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0004LookupPageIdAndDrillDownPageIdDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0004");
    }
}