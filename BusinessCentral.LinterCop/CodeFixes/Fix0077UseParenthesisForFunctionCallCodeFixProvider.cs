using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef;

namespace BusinessCentral.LinterCop.CodeFixes;

[CodeFixProvider("Fix0077UseParenthesisForFunctionCallCodeFixProvider")]
public sealed class Fix0077UseParenthesisForFunctionCallCodeFixProvider : CodeFixProvider
{
    private class Fix0077UseParenthesisForFunctionCallCodeAction : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;
        public override bool SupportsFixAll { get; }

        public Fix0077UseParenthesisForFunctionCallCodeAction(string title,
            Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey, bool generateFixAll)
            : base(title, createChangedDocument, equivalenceKey)
        {
            SupportsFixAll = generateFixAll;
        }
    }

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticDescriptors.Rule0077UseParenthesisForFunctionCall.Id);

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx)
    {
        Document document = ctx.Document;
        TextSpan span = ctx.Span;
        CancellationToken cancellationToken = ctx.CancellationToken;
        SyntaxNode node = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).FindNode(span);

        if (node is not null)
            ctx.RegisterCodeFix(CreateCodeAction(node, document, true), ctx.Diagnostics[0]);
    }

    private static Fix0077UseParenthesisForFunctionCallCodeAction CreateCodeAction(SyntaxNode node, Document document,
        bool generateFixAll)
    {
        return new Fix0077UseParenthesisForFunctionCallCodeAction(
            LinterCopAnalyzers.Fix0077MissingParenthesisCodeAction,
            ct => AddParenthesisForFunction(document, node, ct),
            nameof(Fix0077UseParenthesisForFunctionCallCodeFixProvider),
            generateFixAll);
    }

    private static async Task<Document> AddParenthesisForFunction(Document document, SyntaxNode oldNode, CancellationToken cancellationToken)
    {
        SyntaxNode newNode;
        switch (oldNode.Kind)
        {
            case SyntaxKind.IdentifierName:
                var identifierValue = oldNode.GetIdentifierOrLiteralValue() ?? string.Empty;
                if (string.IsNullOrEmpty(identifierValue))
                    goto default;

                newNode = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(identifierValue)).WithTriviaFrom(oldNode);
                break;

            case SyntaxKind.MemberAccessExpression:
                newNode = SyntaxFactory.InvocationExpression((MemberAccessExpressionSyntax)oldNode);
                break;

            default:
                return document;
        }
        SyntaxNode newRoot = (await document.GetSyntaxRootAsync(cancellationToken)).ReplaceNode(oldNode, newNode);
        return document.WithSyntaxRoot(newRoot);
    }
}