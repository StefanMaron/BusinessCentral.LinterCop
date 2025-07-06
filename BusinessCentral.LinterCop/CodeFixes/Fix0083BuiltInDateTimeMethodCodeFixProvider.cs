using System.Collections.Immutable;
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

        var diagnostic = context.Diagnostics
            .FirstOrDefault(d => d.Id == DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod.Id);

        if (diagnostic is null || !diagnostic.Properties.TryGetValue("ReplacementMethodName", out var replacementMethodName) || string.IsNullOrEmpty(replacementMethodName))
            return;

        context.RegisterCodeFix(
            CreateCodeAction(invocation, context.Document, SyntaxFactory.IdentifierName(replacementMethodName), generateFixAll: true),
            context.Diagnostics[0]);
    }

    private static Fix0083BuiltInDateTimeMethodCodeAction CreateCodeAction(InvocationExpressionSyntax originalNode, Document document, IdentifierNameSyntax replacementMethod, bool generateFixAll)
    {
        return new Fix0083BuiltInDateTimeMethodCodeAction(
            LinterCopAnalyzers.Fix0083BuiltInDateTimeMethodCodeAction,
            ct => ReplaceWithNewMethodAsync(document, originalNode, replacementMethod, ct),
            nameof(Fix0083BuiltInDateTimeMethodCodeFixProvider),
            generateFixAll);
    }

    private static async Task<Document> ReplaceWithNewMethodAsync(Document document, InvocationExpressionSyntax originalNode, IdentifierNameSyntax replacementMethod, CancellationToken cancellationToken)
    {
        if (originalNode.ArgumentList.Arguments.Count == 0)
            return document;

        var firstArgument = originalNode.ArgumentList.Arguments[0];

        var memberAccess = SyntaxFactory.MemberAccessExpression(
            firstArgument,
            replacementMethod);

        var newInvocation = SyntaxFactory.InvocationExpression(memberAccess)
            .WithTriviaFrom(originalNode);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = root.ReplaceNode(originalNode, newInvocation);

        return document.WithSyntaxRoot(newRoot);
    }
}