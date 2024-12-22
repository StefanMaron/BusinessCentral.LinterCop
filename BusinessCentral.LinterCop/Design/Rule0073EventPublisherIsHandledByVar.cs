using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using BusinessCentral.LinterCop.Helpers;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0073EventPublisherIsHandledByVar : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0073EventPublisherIsHandledByVar);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(AnalyzerEventPublisher, SymbolKind.Method);

    private void AnalyzerEventPublisher(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Symbol is not IMethodSymbol methodSymbol)
            return;

        bool hasEventAttribute = methodSymbol.Attributes.Any(attr =>
            attr.AttributeKind == AttributeKind.BusinessEvent ||
            attr.AttributeKind == AttributeKind.IntegrationEvent);
        if (!hasEventAttribute)
            return;

        foreach (var parameter in methodSymbol.Parameters)
        {
            if (!IsInvalidHandledParameter(parameter))
                continue;

            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0073EventPublisherIsHandledByVar,
                parameter.GetLocation()));
        }
    }

    // Verify the "IsHandled" pattern; Must be of type Boolean and passed by var
    private static bool IsInvalidHandledParameter(IParameterSymbol parameter)
    {
        return !parameter.IsVar &&
               parameter.ParameterType.NavTypeKind == NavTypeKind.Boolean &&
               (SemanticFacts.IsSameName(parameter.Name, "IsHandled") ||
                SemanticFacts.IsSameName(parameter.Name, "Handled"));
    }
}