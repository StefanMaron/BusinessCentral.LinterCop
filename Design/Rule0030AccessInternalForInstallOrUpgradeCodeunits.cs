using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using BusinessCentral.LinterCop.AnalysisContextExtension;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    class Rule0030AccessInternalForInstallAndUpgradeCodeunits : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0030AccessInternalForInstallAndUpgradeCodeunits);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckAccessOnInstallAndUpgradeCodeunits), SymbolKind.Codeunit);

        private void CheckAccessOnInstallAndUpgradeCodeunits(SymbolAnalysisContext context)
        {
            if (context.IsObsoletePendingOrRemoved()) return;

            ICodeunitTypeSymbol symbol = (ICodeunitTypeSymbol)context.Symbol;
            if (symbol.Subtype != CodeunitSubtypeKind.Install && symbol.Subtype != CodeunitSubtypeKind.Upgrade)
                return;

            if (symbol.DeclaredAccessibility == Accessibility.Public)
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0030AccessInternalForInstallAndUpgradeCodeunits, symbol.GetLocation()));
        }
    }
}