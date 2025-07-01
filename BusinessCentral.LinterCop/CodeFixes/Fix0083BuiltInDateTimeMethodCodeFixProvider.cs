using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;

namespace BusinessCentral.LinterCop.CodeFixes;

[CodeFixProvider(nameof(Fix0083BuiltInDateTimeMethodCodeFixProvider))]
public sealed class Fix0083BuiltInDateTimeMethodCodeFixProvider : CodeFixProvider
{
    private class Fix0083BuiltInDateTimeMethodCodeAction : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;
        public override bool SupportsFixAll { get; }

        public Fix0083BuiltInDateTimeMethodCodeAction(string title,
            Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey, bool generateFixAll)
            : base(title, createChangedDocument, equivalenceKey)
        {
            SupportsFixAll = generateFixAll;
        }
    }

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod.Id);

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (syntaxRoot.FindNode(context.Span) is not InvocationExpressionSyntax invocation)
            return;

        if (invocation.Expression is not IdentifierNameSyntax identifier)
            return;

        var replacement = Rule0083BuiltInDateTimeMethodHelper.GetReplacementMethod(identifier.Identifier.Text, invocation);
        if (replacement is null)
            return;

        context.RegisterCodeFix(
            CreateCodeAction(invocation, context.Document, replacement, generateFixAll: true),
            context.Diagnostics[0]);
    }

    private static Fix0083BuiltInDateTimeMethodCodeAction CreateCodeAction(InvocationExpressionSyntax originalNode, Document document, InvocationExpressionSyntax replacementNode, bool generateFixAll)
    {
        return new Fix0083BuiltInDateTimeMethodCodeAction(
            LinterCopAnalyzers.Fix0083BuiltInDateTimeMethodCodeAction,
            ct => ReplaceWithNewMethodAsync(document, originalNode, replacementNode, ct),
            nameof(Fix0083BuiltInDateTimeMethodCodeFixProvider),
            generateFixAll);
    }

    private static async Task<Document> ReplaceWithNewMethodAsync(Document document, InvocationExpressionSyntax originalNode, InvocationExpressionSyntax replacementNode, CancellationToken cancellationToken)
    {
        if (originalNode.ArgumentList.Arguments.Count == 0)
            return document;

        var firstArgument = originalNode.ArgumentList.Arguments[0];

        if (replacementNode.Expression is not IdentifierNameSyntax replacementMethodName)
            return document;

        var memberAccess = SyntaxFactory.MemberAccessExpression(
            firstArgument,
            replacementMethodName);

        var newInvocation = SyntaxFactory.InvocationExpression(memberAccess)
            .WithTriviaFrom(originalNode);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = root.ReplaceNode(originalNode, newInvocation);

        return document.WithSyntaxRoot(newRoot);
    }
}