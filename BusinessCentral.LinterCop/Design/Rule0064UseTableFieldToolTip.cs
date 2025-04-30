#if !LessThenSpring2024
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0064UseTableFieldToolTip : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DiagnosticDescriptors.Rule0064TableFieldMissingToolTip,
            DiagnosticDescriptors.Rule0066DuplicateToolTipBetweenPageAndTable);

    public override VersionCompatibility SupportedVersions => VersionCompatibility.Spring2024OrGreater;

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeToolTipProperty),
            SymbolKind.Page,
            SymbolKind.PageExtension);

    private void AnalyzeToolTipProperty(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        var pageFields = GetFlattenedControls(ctx.Symbol)?
                            .Where(e => e.ControlKind == ControlKind.Field &&
                                        e.RelatedFieldSymbol is not null &&
                                        e.GetProperty(PropertyKind.ToolTip) is not null);

        if (pageFields is null || !pageFields.Any())
            return;

        foreach (var pageField in pageFields)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            var pageToolTip = pageField.GetProperty(PropertyKind.ToolTip);
            var tableToolTip = pageField.RelatedFieldSymbol?.GetProperty(PropertyKind.ToolTip);

            if (pageToolTip is null || pageField.RelatedFieldSymbol is null)
                continue;

            // Page field has a value for the ToolTip property and table field does not have a value for the ToolTip property
            if (tableToolTip is null && pageField.RelatedFieldSymbol.IsSourceSymbol())
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0064TableFieldMissingToolTip,
                    pageToolTip.GetLocation(),
                    pageField.RelatedFieldSymbol.Name.QuoteIdentifierIfNeeded(),
                    pageField.Name.QuoteIdentifierIfNeeded()));

                continue;
            }

            // Page field has a value for the ToolTip property and table field also has a value for the ToolTip property but the value is exactly the same
            if (tableToolTip is not null && string.Equals(pageToolTip.ValueText, tableToolTip.ValueText, StringComparison.Ordinal))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0066DuplicateToolTipBetweenPageAndTable,
                    pageToolTip.GetLocation(),
                    pageField.Name.QuoteIdentifierIfNeeded(),
                    pageField.RelatedFieldSymbol.Name.QuoteIdentifierIfNeeded()));

                continue;
            }
        }
    }

    private static IEnumerable<IControlSymbol>? GetFlattenedControls(ISymbol symbol) =>
        symbol switch
        {
            IPageBaseTypeSymbol page => page.FlattenedControls,
            IPageExtensionBaseTypeSymbol pageExtension => pageExtension.AddedControlsFlattened,
            _ => null
        };
}
#endif