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
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(CheckInternalCodeunitForEvents), SymbolKind.Codeunit);
        }

        private void CheckInternalCodeunitForEvents(SymbolAnalysisContext symbolAnalysisContext)
        {
            ICodeunitTypeSymbol codeunitSymbol = symbolAnalysisContext.Symbol as ICodeunitTypeSymbol;
            if (codeunitSymbol == null)
            {
                return;
            }
            if (codeunitSymbol.IsObsoleteRemoved || codeunitSymbol.IsObsoletePending)
            {
                return;
            }
            if (codeunitSymbol.DeclaredAccessibility != Accessibility.Internal)
            {
                return;
            }

            ImmutableArray<ISymbol>.Enumerator enumerator = codeunitSymbol.GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                ISymbol current = enumerator.Current;
                SymbolKind kind = current.Kind;

                if (kind != SymbolKind.Method)
                {
                    continue;
                }

                IMethodSymbol methodSymbol = (IMethodSymbol)current;
                if (!methodSymbol.IsEvent)
                {
                    continue;
                }

                IAttributeSymbol attributeSymbol;
                if (!TryGetEventAttribute(methodSymbol, out attributeSymbol))
                {
                    continue;
                }
                if (attributeSymbol.AttributeKind == AttributeKind.InternalEvent)
                {
                    continue;
                }

                symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0018NoEventsInInternalCodeunitsAnalyzerDescriptor, methodSymbol.GetLocation(), new Object[] { methodSymbol.Name, codeunitSymbol.Name }));
            }
        }

        private bool TryGetEventAttribute(IMethodSymbol method, out IAttributeSymbol attribute)
        {
            ImmutableArray<IAttributeSymbol>.Enumerator enumerator = method.Attributes.GetEnumerator();
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
