using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0060PropertyApplicationAreaOnApiPage : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0060PropertyApplicationAreaOnApiPage);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzePropertyApplicationAreaOnApiPage), SymbolKind.Page);

        private void AnalyzePropertyApplicationAreaOnApiPage(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            if (ctx.Symbol is not IPageTypeSymbol pageTypeSymbol)
                return;

            if (pageTypeSymbol.PageType != PageTypeKind.API)
                return;

            if (pageTypeSymbol.GetProperty(PropertyKind.ApplicationArea) is IPropertySymbol propertyApplicationArea)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0060PropertyApplicationAreaOnApiPage, propertyApplicationArea.GetLocation()));

            IEnumerable<IControlSymbol> pageFields = pageTypeSymbol.FlattenedControls
                                                            .Where(e => e.ControlKind == ControlKind.Field)
                                                            .Where(e => e.GetProperty(PropertyKind.ApplicationArea) is not null);

            foreach (IControlSymbol pageField in pageFields)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0060PropertyApplicationAreaOnApiPage, pageField.GetProperty(PropertyKind.ApplicationArea).GetLocation()));
        }
    }
}