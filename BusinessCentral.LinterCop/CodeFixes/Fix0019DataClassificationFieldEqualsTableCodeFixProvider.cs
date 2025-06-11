using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

namespace BusinessCentral.LinterCop.CodeFixes;

[CodeFixProvider("Fix0019DataClassificationFieldEqualsTableCodeFixProvider")]
public sealed class Fix0019DataClassificationFieldEqualsTableCodeFixProvider : CodeFixProvider
{
    private class Fix0019DataClassificationFieldEqualsTableCodeAction : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;
        public override bool SupportsFixAll { get; }
        public override string? FixAllSingleInstanceTitle => string.Empty;
        public override string? FixAllTitle => Title;

        public Fix0019DataClassificationFieldEqualsTableCodeAction(string title,
            Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey, bool generateFixAll)
            : base(title, createChangedDocument, equivalenceKey)
        {
            SupportsFixAll = generateFixAll;
        }
    }

    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticDescriptors.Rule0019DataClassificationFieldEqualsTable.Id);

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
        SyntaxNode node = syntaxRoot.FindNode(span, getInnermostNodeForTie: true);
        ctx.RegisterCodeFix((CodeAction) CreateCodeAction(node, document, true), ctx.Diagnostics[0]);
    }

    private static Fix0019DataClassificationFieldEqualsTableCodeAction CreateCodeAction(SyntaxNode node, Document document,
        bool generateFixAll)
    {
        return new Fix0019DataClassificationFieldEqualsTableCodeAction(
            LinterCopAnalyzers.Fix0019DataClassificationFieldEqualsTableCodeAction,
            ct => RemoveDataClassificationForField(document, node, ct),
            nameof(Fix0019DataClassificationFieldEqualsTableCodeFixProvider),
            generateFixAll);
    }

    private static async Task<Document> RemoveDataClassificationForField(Document document, SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node.Parent is not PropertyListSyntax originalPropertyList)
            return document;

        PropertyListSyntax? newPropertyList;
        var dataClassificationProperty = originalPropertyList.GetProperty(PropertyKind.DataClassification.ToString());
        if (dataClassificationProperty != null)
        {
            var newProperties = originalPropertyList.Properties.Where(prop => prop != dataClassificationProperty).ToList();
            newPropertyList = SyntaxFactory.PropertyList(SyntaxFactory.List(newProperties));
        } else 
        {
            // If the DataClassification property does not exist, we do not need to change anything.
            return document;
        }

        var newRoot = (await document.GetSyntaxRootAsync(cancellationToken)).ReplaceNode(originalPropertyList, newPropertyList);
        return document.WithSyntaxRoot(newRoot);
    }

}