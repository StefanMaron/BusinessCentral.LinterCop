using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    class Rule0020ApplicationAreaEqualsToPage : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0020ApplicationAreaEqualsToPage);
        public override VersionCompatibility SupportedVersions { get; } = VersionCompatibility.Fall2022OrGreater;

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(CheckDataClassificationRedundancy), SymbolKind.Control);
        }

        private void CheckDataClassificationRedundancy(SymbolAnalysisContext symbolAnalysisContext)
        {
            IControlSymbol Control = (IControlSymbol)symbolAnalysisContext.Symbol;
            if (Control == null || Control.IsObsoleteRemoved || Control.IsObsoletePending)
                return;

            IApplicationObjectTypeSymbol applicationObject = Control.GetContainingApplicationObjectTypeSymbol();
            if (!(applicationObject is IPageTypeSymbol) || applicationObject.IsObsoleteRemoved || applicationObject.IsObsoletePending)
                return;

            IPageTypeSymbol Page = (IPageTypeSymbol)applicationObject;
            IPropertySymbol controlApplicationArea = Control.GetProperty(PropertyKind.ApplicationArea) as IPropertySymbol;
            IPropertySymbol pageApplicationArea = Page.GetProperty(PropertyKind.ApplicationArea) as IPropertySymbol;

            if (controlApplicationArea == null || pageApplicationArea == null)
                return;

            if (pageApplicationArea.ValueText == controlApplicationArea.ValueText)
                symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0020ApplicationAreaEqualsToPage, controlApplicationArea.GetLocation()));
        }

    }
}
