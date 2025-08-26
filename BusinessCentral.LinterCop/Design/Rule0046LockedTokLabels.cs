using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

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

        bool IsEndsWithTok = type.Name?.EndsWith("Tok", StringComparison.Ordinal) == true;
        bool isLocked = type.Locked is true;
        if (isLocked && !IsEndsWithTok)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0047LockedLabelsTok,
                symbol.GetLocation(), symbol.Name));
        }

        if (!isLocked && IsEndsWithTok)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0046TokLabelsLocked,
                symbol.GetLocation()));
        }
    }
}