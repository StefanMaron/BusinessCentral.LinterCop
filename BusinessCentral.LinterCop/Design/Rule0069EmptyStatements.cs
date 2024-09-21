#nullable enable
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0069EmptyStatements : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0069EmptyStatements);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeEmptyStatement), OperationKind.EmptyStatement);

        private void AnalyzeEmptyStatement(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            foreach (SyntaxTrivia trivia in ctx.Operation.Syntax.Parent.GetLeadingTrivia())
                if (trivia.IsKind(SyntaxKind.LineCommentTrivia))
                    return;

            foreach (SyntaxTrivia trivia in ctx.Operation.Syntax.Parent.GetTrailingTrivia())
                if (trivia.IsKind(SyntaxKind.LineCommentTrivia))
                    return;

            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0069EmptyStatements, ctx.Operation.Syntax.GetLocation()));
        }
    }
}