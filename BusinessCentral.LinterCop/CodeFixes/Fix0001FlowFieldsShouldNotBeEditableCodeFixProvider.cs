using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;

namespace BusinessCentral.LinterCop.CodeFixes;

[Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef.CodeFixProvider("Fix0001FlowFieldsShouldNotBeEditableCodeFixProvider")]
public sealed class Fix0001FlowFieldsShouldNotBeEditableCodeFixProvider : CodeFixProvider
{
    private class Fix0001FlowFieldsShouldNotBeEditableCodeAction : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;
        public override bool SupportsFixAll { get; }
        public override string? FixAllSingleInstanceTitle => string.Empty;
        public override string? FixAllTitle => Title;
        public Fix0001FlowFieldsShouldNotBeEditableCodeAction(string title, Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey, bool generateFixAll)
            : base(title, createChangedDocument, equivalenceKey)
        {
            SupportsFixAll = generateFixAll;
        }
    }

    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable.Id);

    public sealed override FixAllProvider GetFixAllProvider() =>
         WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx)
    {
        Document document = ctx.Document;
        TextSpan span = ctx.Span;
        CancellationToken cancellationToken = ctx.CancellationToken;

        SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        RegisterInstanceCodeFix(ctx, syntaxRoot, span, document);
    }

    private static void RegisterInstanceCodeFix(CodeFixContext ctx, SyntaxNode syntaxRoot, TextSpan span, Document document)
    {
        SyntaxNode node = syntaxRoot.FindNode(span);
        ctx.RegisterCodeFix(CreateCodeAction(node, document, true), ctx.Diagnostics[0]);
    }

    private static Fix0001FlowFieldsShouldNotBeEditableCodeAction CreateCodeAction(SyntaxNode node, Document document, bool generateFixAll) =>
        new(LinterCopAnalyzers.Fix0001FlowFieldsShouldNotBeEditableCodeAction, ct => SetEditablePropertyForField(document, node, ct), "Fix0001FlowFieldsShouldNotBeEditableCodeFixProvider", generateFixAll);

    private static async Task<Document> SetEditablePropertyForField(Document document, SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node.Parent is not FieldSyntax originalFieldNode)
            return document;

        FieldSyntax newFieldNode;
        var editableProperty = originalFieldNode.GetProperty(PropertyKind.Editable.ToString());
        if (editableProperty is null)
        {
            newFieldNode = originalFieldNode.AddPropertyListProperties(GetEditableFalseProperty());
        }
        else
        {
            var newPropertyList = UpdateEditablePropertyList(originalFieldNode, editableProperty);
            newFieldNode = originalFieldNode.WithPropertyList(newPropertyList);
        }

        var newRoot = (await document.GetSyntaxRootAsync(cancellationToken)).ReplaceNode(originalFieldNode, newFieldNode);
        return document.WithSyntaxRoot(newRoot);
    }

    private static PropertySyntax GetEditableFalseProperty() =>
        SyntaxFactory.Property(PropertyKind.Editable, GetBooleanFalsePropertyValue());

    private static PropertyListSyntax UpdateEditablePropertyList(FieldSyntax fieldNode, PropertySyntax editableProperty)
    {
        var updatedEditableProperty = editableProperty.WithValue(GetBooleanFalsePropertyValue());

        var propertyList = fieldNode.PropertyList;
        var newProperties = propertyList.Properties.Select(prop =>
            prop == editableProperty ? updatedEditableProperty : prop).ToList();

        return propertyList.WithProperties(SyntaxFactory.List(newProperties));
    }

    private static PropertyValueSyntax GetBooleanFalsePropertyValue() =>
        SyntaxFactory.BooleanPropertyValue(SyntaxFactory.BooleanLiteralValue(SyntaxFactory.Token(SyntaxKind.FalseKeyword)));
}