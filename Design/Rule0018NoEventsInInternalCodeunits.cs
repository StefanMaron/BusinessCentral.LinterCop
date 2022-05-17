using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    class Rule0018NoEventsInInternalCodeunits : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0018NoEventsInInternalCodeunitsAnalyzerDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(CheckPublicEventInInternalCodeunit), SymbolKind.Method);
        }

        private void CheckPublicEventInInternalCodeunit(SymbolAnalysisContext symbolAnalysisContext)
        {
            IMethodSymbol methodSymbol = symbolAnalysisContext.Symbol as IMethodSymbol;
            if (methodSymbol == null || !methodSymbol.IsEvent || methodSymbol.IsObsoleteRemoved || methodSymbol.IsObsoletePending)
                return;

            IApplicationObjectTypeSymbol applicationObject = methodSymbol.GetContainingApplicationObjectTypeSymbol();
            if (!(applicationObject is ICodeunitTypeSymbol) || applicationObject.DeclaredAccessibility != Accessibility.Internal || applicationObject.IsObsoleteRemoved || applicationObject.IsObsoletePending)
                return;

            IAttributeSymbol attributeSymbol;
            if (!TryGetEventAttribute(methodSymbol, out attributeSymbol) || attributeSymbol.AttributeKind == AttributeKind.InternalEvent)
                return;

            symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0018NoEventsInInternalCodeunitsAnalyzerDescriptor, methodSymbol.GetLocation(), new Object[] { methodSymbol.Name, applicationObject.Name }));
        }

        private bool TryGetEventAttribute(IMethodSymbol methodSymbol, out IAttributeSymbol attribute)
        {
            ImmutableArray<IAttributeSymbol>.Enumerator enumerator = methodSymbol.Attributes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IAttributeSymbol current = enumerator.Current;
                AttributeKind attributeKind = current.AttributeKind;
                if (attributeKind == AttributeKind.IntegrationEvent)
                {
                    attribute = current;
                    return true;
                }
            }
            attribute = null;
            return false;
        }
    }
}
