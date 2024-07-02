using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0025InternalProcedureModifier : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0025InternalProcedureModifier);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeInternalProcedures), SyntaxKind.MethodDeclaration);

        private void AnalyzeInternalProcedures(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().DeclaredAccessibility != Accessibility.Public) return;

            MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)ctx.Node;
            SyntaxNodeOrToken accessModifier = methodDeclarationSyntax.ProcedureKeyword.GetPreviousToken();

            if (accessModifier.Kind == SyntaxKind.LocalKeyword || accessModifier.Kind == SyntaxKind.InternalKeyword) return;

            if (methodDeclarationSyntax.GetLeadingTrivia().Where(x => x.Kind == SyntaxKind.SingleLineDocumentationCommentTrivia).Any()) return;

            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0025InternalProcedureModifier, methodDeclarationSyntax.ProcedureKeyword.GetLocation()));
        }
    }
}