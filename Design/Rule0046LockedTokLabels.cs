using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0046LockedTokLabels : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0046TokLabelsLocked, DiagnosticDescriptors.Rule0047LockedLabelsTok);
        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeLockedLabel), SymbolKind.GlobalVariable, SymbolKind.LocalVariable);

        private void AnalyzeLockedLabel(SymbolAnalysisContext ctx)
        {
            IVariableSymbol symbol = (IVariableSymbol)ctx.Symbol;

            ITypeSymbol type1 = symbol.Type;
            if (type1 == null || type1.NavTypeKind != NavTypeKind.Label)
                return;

            ILabelTypeSymbol type = symbol.Type as ILabelTypeSymbol;
            if (type.Locked)
                if (!type.Name.EndsWith("Tok"))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0047LockedLabelsTok, symbol.GetLocation(), symbol.Name));

            if (type.Name.EndsWith("Tok"))
                if (!type.Locked)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0046TokLabelsLocked, symbol.GetLocation()));

        }
    }
}