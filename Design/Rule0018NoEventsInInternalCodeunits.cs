using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design {
    [DiagnosticAnalyzer]
    class Rule0018NoEventsInInternalCodeunits : DiagnosticAnalyzer {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0018NoEventsInInternalCodeunitsAnalyzerDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(CheckPublicEventInInternalCodeunit), SymbolKind.Method);
        }

        private void CheckPublicEventInInternalCodeunit(SymbolAnalysisContext symbolAnalysisContext)
        {
            IMethodSymbol methodSymbol = symbolAnalysisContext.Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return;
            }
            if (methodSymbol.IsObsoleteRemoved || methodSymbol.IsObsoletePending)
            {
                return;
            }
            if (!methodSymbol.IsEvent)
            {
                return;
            }

            IApplicationObjectTypeSymbol applicationObject = methodSymbol.GetContainingApplicationObjectTypeSymbol();
            if (!(applicationObject is ICodeunitTypeSymbol))
            {
                return;
            }
            if (applicationObject.IsObsoleteRemoved || applicationObject.IsObsoletePending)
            {
                return;
            }
            if (applicationObject.DeclaredAccessibility != Accessibility.Internal)
            {
                return;
            }

            IAttributeSymbol attributeSymbol;
            if (!TryGetEventAttribute(methodSymbol, out attributeSymbol))
            {
                return;
            }
            if (attributeSymbol.AttributeKind == AttributeKind.InternalEvent)
            {
                return;
            }

            symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0018NoEventsInInternalCodeunitsAnalyzerDescriptor, methodSymbol.GetLocation(), new Object[] { methodSymbol.Name, applicationObject.Name }));
        }

        private bool TryGetEventAttribute(IMethodSymbol methodSymbol, out IAttributeSymbol attribute)
        {
            ImmutableArray<IAttributeSymbol>.Enumerator enumerator = methodSymbol.Attributes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IAttributeSymbol current = enumerator.Current;
                AttributeKind attributeKind = current.AttributeKind;
                if (attributeKind != AttributeKind.BusinessEvent && (int)attributeKind - (int)AttributeKind.IntegrationEvent > (int)AttributeKind.Caption)
                {
                    continue;
                }
                attribute = current;
                return true;
            }
            attribute = null;
            return false;
        }
    }
}
