#if Spring2023
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0028CodeNavigabilityOnEventSubscribers : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0028IdentifiersInEventSubscribers);

        public override void Initialize(AnalysisContext context) => context.RegisterCodeBlockAction(new Action<CodeBlockAnalysisContext>(this.AnalyzeIdentifiersInEventSubscribers));

        private void AnalyzeIdentifiersInEventSubscribers(CodeBlockAnalysisContext context)
        {
            if (!VersionChecker.IsSupported(context.OwningSymbol, Feature.IdentifiersInEventSubscribers)) return;

            if (context.OwningSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || context.OwningSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (context.OwningSymbol.IsObsoletePending || context.OwningSymbol.IsObsoleteRemoved) return;

            if (!context.CodeBlock.IsKind(SyntaxKind.MethodDeclaration)) return;

            var SyntaxList = ((MethodDeclarationSyntax)context.CodeBlock).Attributes.Where(value => SemanticFacts.IsSameName(value.GetIdentifierOrLiteralValue(), "EventSubscriber"));

            if (SyntaxList.Where(value => value.ArgumentList.Arguments[2].IsKind(SyntaxKind.LiteralAttributeArgument)).Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0028IdentifiersInEventSubscribers, context.OwningSymbol.GetLocation()));
                return;
            }

            if (SyntaxList.Where(value => !(value.ArgumentList.Arguments[3].GetIdentifierOrLiteralValue() == "") && value.ArgumentList.Arguments[3].IsKind(SyntaxKind.LiteralAttributeArgument)).Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0028IdentifiersInEventSubscribers, context.OwningSymbol.GetLocation()));
                return;
            }
        }
    }
}
#endif