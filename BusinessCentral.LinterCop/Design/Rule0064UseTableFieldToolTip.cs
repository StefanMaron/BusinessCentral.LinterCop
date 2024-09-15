#nullable disable // TODO: Enable nullable and review rule
#if Spring2024OrGreater
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0064UseTableFieldToolTip : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0064TableFieldMissingToolTip, DiagnosticDescriptors.Rule0066DuplicateToolTipBetweenPageAndTable);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeFlowFieldEditable), SymbolKind.Page, SymbolKind.PageExtension);

        private void AnalyzeFlowFieldEditable(SymbolAnalysisContext ctx)
        {
            if (!VersionChecker.IsSupported(ctx.Symbol, VersionCompatibility.Spring2024OrGreater)) return;

            if (ctx.IsObsoletePendingOrRemoved()) return;

            IEnumerable<IControlSymbol> pageFields = GetFlattenedControls(ctx.Symbol)
                                                        .Where(e => e.ControlKind == ControlKind.Field)
                                                        .Where(e => e.GetProperty(PropertyKind.ToolTip) != null)
                                                        .Where(e => e.RelatedFieldSymbol != null);
            if (pageFields == null) return;

            foreach (IControlSymbol pageField in pageFields)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                IPropertySymbol pageToolTip = pageField.GetProperty(PropertyKind.ToolTip);
                IPropertySymbol tableToolTip = pageField.RelatedFieldSymbol.GetProperty(PropertyKind.ToolTip);

                // Page field has a value for the ToolTip property and table field does not have a value for the ToolTip property
                if (tableToolTip == null && pageField.RelatedFieldSymbol.IsSourceSymbol())
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0064TableFieldMissingToolTip, pageToolTip.GetLocation(), new object[] { pageField.RelatedFieldSymbol.Name.QuoteIdentifierIfNeeded(), pageField.Name.QuoteIdentifierIfNeeded() }));
                    continue;
                }

                // Page field has a value for the ToolTip property and table field also has a value for the ToolTip property but the value is exactly the same
                if (tableToolTip != null && pageToolTip.ValueText == tableToolTip.ValueText)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0066DuplicateToolTipBetweenPageAndTable, pageToolTip.GetLocation(), new object[] { pageField.Name.QuoteIdentifierIfNeeded(), pageField.RelatedFieldSymbol.Name.QuoteIdentifierIfNeeded() }));
                    continue;
                }
            }
        }

        private static IEnumerable<IControlSymbol> GetFlattenedControls(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Page:
                    return ((IPageBaseTypeSymbol)symbol).FlattenedControls;
                case SymbolKind.PageExtension:
                    return ((IPageExtensionBaseTypeSymbol)symbol).AddedControlsFlattened;
                default:
                    return null;
            }
        }

        public static class DiagnosticDescriptors
        {
            public static readonly DiagnosticDescriptor Rule0064TableFieldMissingToolTip = new(
                id: LinterCopAnalyzers.AnalyzerPrefix + "0064",
                title: LinterCopAnalyzers.GetLocalizableString("Rule0064TableFieldMissingToolTipTitle"),
                messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0064TableFieldMissingToolTipFormat"),
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
                description: LinterCopAnalyzers.GetLocalizableString("Rule0064TableFieldMissingToolTipDescription"),
                helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0064");

            public static readonly DiagnosticDescriptor Rule0066DuplicateToolTipBetweenPageAndTable = new(
                id: LinterCopAnalyzers.AnalyzerPrefix + "0066",
                title: LinterCopAnalyzers.GetLocalizableString("Rule0066DuplicateToolTipBetweenPageAndTableTitle"),
                messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0066DuplicateToolTipBetweenPageAndTableFormat"),
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
                description: LinterCopAnalyzers.GetLocalizableString("Rule0066DuplicateToolTipBetweenPageAndTableDescription"),
                helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0066");
        }
    }
}
#endif