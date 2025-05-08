using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0030AccessInternalForInstallAndUpgradeCodeunits : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0030AccessInternalForInstallAndUpgradeCodeunits);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckAccessOnInstallAndUpgradeCodeunits), SymbolKind.Codeunit);

    private void CheckAccessOnInstallAndUpgradeCodeunits(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not ICodeunitTypeSymbol symbol)
            return;

        if (symbol.Subtype != CodeunitSubtypeKind.Install && symbol.Subtype != CodeunitSubtypeKind.Upgrade)
            return;

        if (symbol.DeclaredAccessibility == Accessibility.Public)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0030AccessInternalForInstallAndUpgradeCodeunits,
                symbol.GetLocation()));
    }
}
