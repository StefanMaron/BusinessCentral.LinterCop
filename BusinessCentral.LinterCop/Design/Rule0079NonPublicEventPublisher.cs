using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0079NonPublicEventPublisher : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0079NonPublicEventPublisher);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(AnalyzeEventPublisher, SymbolKind.Method);

    private void AnalyzeEventPublisher(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IMethodSymbol symbol)
            return;

        if (symbol.IsEvent && !symbol.IsLocal && !symbol.IsInternal)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0079NonPublicEventPublisher, symbol.GetLocation()));
    }
}