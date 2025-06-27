using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;

namespace BusinessCentral.LinterCop.CodeFixes;

[CodeFixProvider(nameof(Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeFixProvider))]
public sealed class Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeFixProvider : CodeFixProvider
{
    private class Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeAction : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;
        public override bool SupportsFixAll { get; }
        public override string? FixAllSingleInstanceTitle => string.Empty;
        public override string? FixAllTitle => Title;

        public Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeAction(string title,
            Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey, bool generateFixAll)
            : base(title, createChangedDocument, equivalenceKey)
        {
            SupportsFixAll = generateFixAll;
        }
    }

    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticDescriptors.Rule0024SemicolonAfterMethodOrTriggerDeclaration.Id);

    public sealed override FixAllProvider GetFixAllProvider() =>
         WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx)
    {
        Document document = ctx.Document;
        TextSpan span = ctx.Span;
        CancellationToken cancellationToken = ctx.CancellationToken;

        SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
        RegisterInstanceCodeFix(ctx, syntaxRoot, span, document);
    }

    private static void RegisterInstanceCodeFix(CodeFixContext ctx, SyntaxNode syntaxRoot, TextSpan span, Document document)
    {
        SyntaxNode node = syntaxRoot.FindNode(span);
        ctx.RegisterCodeFix((CodeAction)CreateCodeAction(node, document, true), ctx.Diagnostics[0]);
    }

    private static Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeAction CreateCodeAction(SyntaxNode node, Document document,
        bool generateFixAll)
    {
        return new Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeAction(
            LinterCopAnalyzers.Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeAction,
            ct => RemoveSemicolon(document, node, ct),
            nameof(Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeFixProvider),
            generateFixAll);
    }

    private static async Task<Document> RemoveSemicolon(Document document, SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node is not MethodOrTriggerDeclarationSyntax syntax)
            return document;
        if (syntax.Body is null)
            return document;

        if (syntax.SemicolonToken.Kind != SyntaxKind.None)
        {
            var trailingTrivia = syntax.SemicolonToken.TrailingTrivia;
            var prevToken = syntax.SemicolonToken.GetPreviousToken();
            var newPrevToken = prevToken.WithTrailingTrivia(trailingTrivia);
            var newNode = syntax.ReplaceTokens(new[] { syntax.SemicolonToken, prevToken }, (original, _) =>
            {
                if (original == syntax.SemicolonToken)
                    return default(SyntaxToken); // removes the token
                if (original == prevToken)
                    return newPrevToken;
                return original;
            });


            var newRoot = (await document.GetSyntaxRootAsync(cancellationToken)).ReplaceNode(node, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
        return document;
    }


}