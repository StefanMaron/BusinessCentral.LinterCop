using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0024SemicolonAfterMethodOrTriggerDeclaration : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0024SemicolonAfterMethodOrTriggerDeclaration);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeSemicolonAfterMethodOrTriggerDeclaration), SyntaxKind.MethodDeclaration, SyntaxKind.TriggerDeclaration);

        private void AnalyzeSemicolonAfterMethodOrTriggerDeclaration(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;
#if Spring2021
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
#endif
            MethodOrTriggerDeclarationSyntax syntax = ctx.Node as MethodOrTriggerDeclarationSyntax;

            if (syntax.SemicolonToken.Kind != SyntaxKind.None)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0024SemicolonAfterMethodOrTriggerDeclaration, syntax.SemicolonToken.GetLocation()));
            }
        }
    }
}
