using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0046LockedTokLabels : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0046TokLabelsLocked, DiagnosticDescriptors.Rule0047LockedLabelsTok);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeLockedLabel),
            SymbolKind.GlobalVariable,
            SymbolKind.LocalVariable);

    private void AnalyzeLockedLabel(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IVariableSymbol symbol)
            return;

        if (symbol.Type is not ILabelTypeSymbol type)
            return;

        if (type.Locked)
            if (!type.Name.EndsWith("Tok"))
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0047LockedLabelsTok,
                    symbol.GetLocation(), symbol.Name));

        if (type.Name.EndsWith("Tok"))
            if (!type.Locked)
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0046TokLabelsLocked,
                    symbol.GetLocation()));
    }
}