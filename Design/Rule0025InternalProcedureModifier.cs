using System;
using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0025InternalProcedureModifier : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0025InternalProcedureModifier);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeInternalProcedures), SyntaxKind.MethodDeclaration);

        private void AnalyzeInternalProcedures(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;

            MethodDeclarationSyntax syntax = ctx.Node as MethodDeclarationSyntax;
            SyntaxNodeOrToken accessModifier = syntax.ProcedureKeyword.GetPreviousToken();

            if (accessModifier.Kind != SyntaxKind.LocalKeyword && accessModifier.Kind != SyntaxKind.InternalKeyword)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0025InternalProcedureModifier, syntax.ProcedureKeyword.GetLocation()));

        }
    }
}