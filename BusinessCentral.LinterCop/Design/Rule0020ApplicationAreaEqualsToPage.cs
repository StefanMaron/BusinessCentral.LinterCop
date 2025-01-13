using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0020ApplicationAreaEqualsToPage : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0020ApplicationAreaEqualsToPage);
    public override VersionCompatibility SupportedVersions { get; } = VersionCompatibility.Fall2022OrGreater;

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(CheckDataClassificationRedundancy), SymbolKind.Control);

    private void CheckDataClassificationRedundancy(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IControlSymbol control)
            return;

        IApplicationObjectTypeSymbol? applicationObject = control.GetContainingApplicationObjectTypeSymbol();
        if (applicationObject is not IPageTypeSymbol page || applicationObject.IsObsoletePendingOrRemoved())
            return;

        IPropertySymbol? controlApplicationArea = control.GetProperty(PropertyKind.ApplicationArea);
        if (controlApplicationArea is null)
            return;

        IPropertySymbol? pageApplicationArea = page.GetProperty(PropertyKind.ApplicationArea);
        if (pageApplicationArea is null)
            return;

        if (pageApplicationArea.ValueText == controlApplicationArea.ValueText)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0020ApplicationAreaEqualsToPage,
                controlApplicationArea.GetLocation()));
    }
}