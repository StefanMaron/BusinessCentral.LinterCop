using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0079NonPublicEventPublisher : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0079NonPublicEventPublisher);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(AnalyzeEventPublisher, SymbolKind.Method);

    private void AnalyzeEventPublisher(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Symbol is not IMethodSymbol symbol || !symbol.IsEvent)
            return;

        if (!symbol.IsLocal && !symbol.IsInternal)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0079NonPublicEventPublisher, symbol.GetLocation()));
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0079NonPublicEventPublisher = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0079",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0079NonPublicEventPublisherTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0079NonPublicEventPublisherFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0079NonPublicEventPublisherDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0079");
    }
}
