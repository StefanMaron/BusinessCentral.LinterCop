using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0064UseTableFieldToolTipInsteadOfPageFieldToolTip : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0064UseTableFieldToolTipInsteadOfPageFieldToolTip);

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

            foreach (IControlSymbol page in pageFields)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                IPropertySymbol pageToolTip = page.GetProperty(PropertyKind.ToolTip);
                IPropertySymbol tableToolTip = page.RelatedFieldSymbol.GetProperty(PropertyKind.ToolTip);

                // Page field has a value for the ToolTip property and table field does not have a value for the ToolTip property
                if (tableToolTip == null && page.RelatedFieldSymbol.IsSourceSymbol())
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0064UseTableFieldToolTipInsteadOfPageFieldToolTip, pageToolTip.GetLocation()));
                    continue;
                }

                // Page field has a value for the ToolTip property and table field also has a value for the ToolTip property but the value is exactly the same
                if (tableToolTip != null && pageToolTip.ValueText == tableToolTip.ValueText)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0064UseTableFieldToolTipInsteadOfPageFieldToolTip, pageToolTip.GetLocation()));
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
    }
}
