using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0002CommitMustBeExplainedByComment : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0002CommitMustBeExplainedByComment);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckCommitForExplainingComment), OperationKind.InvocationExpression);

        private void CheckCommitForExplainingComment(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.Name.ToUpper() == "COMMIT" && operation.TargetMethod.MethodKind == MethodKind.BuiltInMethod)
            {
                foreach (SyntaxTrivia trivia in operation.Syntax.Parent.GetLeadingTrivia())
                {
                    if (trivia.IsKind(SyntaxKind.LineCommentTrivia))
                    {
                        return;
                    }
                }
                foreach (SyntaxTrivia trivia in operation.Syntax.Parent.GetTrailingTrivia())
                {
                    if (trivia.IsKind(SyntaxKind.LineCommentTrivia))
                    {
                        return;
                    }
                }

                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0002CommitMustBeExplainedByComment, ctx.Operation.Syntax.GetLocation()));
            }
        }
    }
}
