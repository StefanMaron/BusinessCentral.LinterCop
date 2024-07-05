#nullable enable
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0060CheckEventSubscriberVarKeyword : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0060EventSubscriberVarCheck);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSymbolAction(CheckForEventSubscriberVar, SymbolKind.Method);
    }

    private void CheckForEventSubscriberVar(SymbolAnalysisContext context)
    {
        var methodSymbol = (IMethodSymbol)context.Symbol;
        var eventSubscriberAttribute = methodSymbol.Attributes
            .FirstOrDefault(attr => attr.AttributeKind == AttributeKind.EventSubscriber);

        if (eventSubscriberAttribute == null)
        {
            return;
        }

        var method = GetReferencedEventPublisherMethodSymbol(context, eventSubscriberAttribute);
        if (method == null)
        {
            return;
        }

        var publisherParameters = method.Parameters;

        foreach (var subscriberParameter in methodSymbol.Parameters)
        {
            var publisherParameter = publisherParameters.FirstOrDefault(p => p.Name.Equals(subscriberParameter.Name, StringComparison.OrdinalIgnoreCase));
            if (publisherParameter == null)
            {
                continue;
            }

            if (publisherParameter.IsVar && !subscriberParameter.IsVar)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0060EventSubscriberVarCheck,
                    subscriberParameter.GetLocation(),
                    new object[] { subscriberParameter.Name}));
            }
        }
    }

    private static IMethodSymbol? GetReferencedEventPublisherMethodSymbol(SymbolAnalysisContext context, IAttributeSymbol eventSubscriberAttribute)
    {
        var applicationObject = eventSubscriberAttribute.GetReferencedApplicationObject();
        if (applicationObject == null)
        {
            return null;
        }

        if (eventSubscriberAttribute.Arguments.Length < 3)
        {
            return null;
        }
        
        var eventName = eventSubscriberAttribute.Arguments[2].ValueText; 
        return applicationObject.GetFirstMethod(eventName, context.Compilation);
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0060EventSubscriberVarCheck = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0060",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0060EventSubscriberVarCheckTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0060EventSubscriberVarCheckFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description:  LinterCopAnalyzers.GetLocalizableString("Rule0060EventSubscriberVarCheckDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0060");
    }
}