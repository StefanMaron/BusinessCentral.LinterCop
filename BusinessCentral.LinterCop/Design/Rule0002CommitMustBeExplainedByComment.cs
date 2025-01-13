using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0002CommitMustBeExplainedByComment : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0002CommitMustBeExplainedByComment);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckCommitForExplainingComment), OperationKind.InvocationExpression);

    private void CheckCommitForExplainingComment(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.TargetMethod.Name != "Commit")
            return;

        var parentSyntax = operation.Syntax.Parent;
        if (HasLineComment(parentSyntax.GetLeadingTrivia()) || HasLineComment(parentSyntax.GetTrailingTrivia()))
            return;

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0002CommitMustBeExplainedByComment,
            ctx.Operation.Syntax.GetLocation()));
    }
    private static bool HasLineComment(SyntaxTriviaList triviaList)
    {
        return triviaList.Any(trivia => trivia.IsKind(SyntaxKind.LineCommentTrivia));
    }
}
