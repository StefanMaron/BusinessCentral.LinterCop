using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0024SemicolonAfterMethodOrTriggerDeclaration : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0024SemicolonAfterMethodOrTriggerDeclaration);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeSemicolonAfterMethodOrTriggerDeclaration),
            SyntaxKind.MethodDeclaration,
            SyntaxKind.TriggerDeclaration);

    private void AnalyzeSemicolonAfterMethodOrTriggerDeclaration(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Node is not MethodOrTriggerDeclarationSyntax syntax)
            return;

        if (syntax.SemicolonToken.Kind != SyntaxKind.None)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0024SemicolonAfterMethodOrTriggerDeclaration,
                syntax.SemicolonToken.GetLocation()));
        }
    }
}