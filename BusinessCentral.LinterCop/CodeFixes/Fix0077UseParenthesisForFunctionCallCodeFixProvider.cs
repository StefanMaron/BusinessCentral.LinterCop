using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;

namespace BusinessCentral.LinterCop.CodeFixes;

[Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef.CodeFixProvider("Fix0077UseParenthesisForFunctionCallCodeFixProvider")]
public sealed class Fix0077UseParenthesisForFunctionCallCodeFixProvider : CodeFixProvider
{
    private class UseParenthesisForFunctionCallCodeAction : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;

        public UseParenthesisForFunctionCallCodeAction(string title, Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey)
            : base(title, createChangedDocument, equivalenceKey)
        {
        }
    }

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticDescriptors.Rule0077UseParenthesisForFunctionCall.Id);

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Document document = context.Document;
        TextSpan span = context.Span;
        CancellationToken cancellationToken = context.CancellationToken;
        SyntaxNode node = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).FindNode(span);
        if (node != null)
        {
            UseParenthesisForFunctionCallCodeAction useParenthesisForFunctionCallCodeAction = new UseParenthesisForFunctionCallCodeAction(LinterCopAnalyzers.Fix0077MissingParenthesisCodeAction, (CancellationToken c) => AddParenthesisForFunction(document, node, c), "UseParenthesisForFunctionCallCodeAction");
            context.RegisterCodeFix(useParenthesisForFunctionCallCodeAction, context.Diagnostics);
        }
    }

    private async Task<Document> AddParenthesisForFunction(Document document, SyntaxNode oldNode, CancellationToken cancellationToken)
    {
        SyntaxNode newNode;
        switch (oldNode.Kind)
        {
            case SyntaxKind.MemberAccessExpression:
                newNode = SyntaxFactory.InvocationExpression((MemberAccessExpressionSyntax)oldNode);
                break;
            case SyntaxKind.IdentifierName:
                var identifierValue = oldNode.GetIdentifierOrLiteralValue() ?? string.Empty;
                if (string.IsNullOrEmpty(identifierValue))
                    goto default;

                newNode = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(identifierValue)).WithTriviaFrom(oldNode);
                break;
            default:
                return document;
        }
        SyntaxNode newRoot = (await document.GetSyntaxRootAsync(cancellationToken)).ReplaceNode(oldNode, newNode);
        return document.WithSyntaxRoot(newRoot);
    }
}