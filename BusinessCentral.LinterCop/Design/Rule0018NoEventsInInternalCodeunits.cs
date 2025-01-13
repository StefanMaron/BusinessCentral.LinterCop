using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0018NoEventsInInternalCodeunits : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0018NoEventsInInternalCodeunitsAnalyzerDescriptor);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(CheckPublicEventInInternalCodeunit), SymbolKind.Method);

    private void CheckPublicEventInInternalCodeunit(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IMethodSymbol methodSymbol)
            return;

        if (!methodSymbol.IsEvent)
            return;

        IApplicationObjectTypeSymbol? applicationObject = methodSymbol.GetContainingApplicationObjectTypeSymbol();
        if (applicationObject is null || !IsInternalCodeunit(applicationObject))
            return;

        IAttributeSymbol? attributeSymbol;
        if (!TryGetEventAttribute(methodSymbol, out attributeSymbol) || attributeSymbol?.AttributeKind == AttributeKind.InternalEvent)
            return;

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0018NoEventsInInternalCodeunitsAnalyzerDescriptor,
            methodSymbol.GetLocation(),
            methodSymbol.Name,
            applicationObject.Name));
    }

    private bool IsInternalCodeunit(IApplicationObjectTypeSymbol applicationObject) =>
        applicationObject is ICodeunitTypeSymbol &&
        applicationObject.DeclaredAccessibility == Accessibility.Internal &&
        !applicationObject.IsObsoletePendingOrRemoved();

    private bool TryGetEventAttribute(IMethodSymbol methodSymbol, out IAttributeSymbol? attribute)
    {
        attribute = methodSymbol.Attributes.FirstOrDefault(attr => attr.AttributeKind == AttributeKind.IntegrationEvent);
        return attribute is not null;
    }
}