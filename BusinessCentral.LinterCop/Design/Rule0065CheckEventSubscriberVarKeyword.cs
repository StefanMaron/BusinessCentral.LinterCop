using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using BusinessCentral.LinterCop.Helpers;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0065CheckEventSubscriberVarKeyword : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0065EventSubscriberVarCheck);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(CheckForEventSubscriberVar, SymbolKind.Method);

    private void CheckForEventSubscriberVar(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IMethodSymbol methodSymbol)
            return;

        var eventSubscriberAttribute = methodSymbol.Attributes
            .FirstOrDefault(attr => attr.AttributeKind == AttributeKind.EventSubscriber);

        if (eventSubscriberAttribute is null)
            return;

        var method = GetReferencedEventPublisherMethodSymbol(ctx, eventSubscriberAttribute);
        if (method is null)
            return;

        var publisherParameters = method.Parameters.ToDictionary(
            p => p.Name,
            StringComparer.OrdinalIgnoreCase);

        foreach (var subscriberParameter in methodSymbol.Parameters)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            if (publisherParameters.TryGetValue(subscriberParameter.Name, out var publisherParameter))
            {
                if (publisherParameter.IsVar && !subscriberParameter.IsVar)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.Rule0065EventSubscriberVarCheck,
                        subscriberParameter.GetLocation(),
                        subscriberParameter.Name));
                }
            }
        }
    }

    private static IMethodSymbol? GetReferencedEventPublisherMethodSymbol(SymbolAnalysisContext context, IAttributeSymbol eventSubscriberAttribute)
    {
        if (context.CancellationToken.IsCancellationRequested)
            return null;

        var applicationObject = eventSubscriberAttribute.GetReferencedApplicationObject();
        if (applicationObject is null || eventSubscriberAttribute.Arguments.Length < 3)
            return null;

        var eventName = eventSubscriberAttribute.Arguments[2].ValueText;
        if (string.IsNullOrEmpty(eventName))
            return null;

        return applicationObject.GetFirstMethod(eventName, context.Compilation);
    }
}