using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;

namespace BusinessCentral.LinterCop.CodeFixes;

[CodeFixProvider(nameof(Fix0051AppendMaxLengthToLabelCodeFixProvider))]
public sealed class Fix0051AppendMaxLengthToLabelCodeFixProvider : CodeFixProvider
{
    private class Fix0051AppendMaxLengthToLabelCodeAction : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;
        public override bool SupportsFixAll { get; }
        public override string? FixAllSingleInstanceTitle => string.Empty;
        public override string? FixAllTitle => Title;

        public Fix0051AppendMaxLengthToLabelCodeAction(string title,
            Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey, bool generateFixAll)
            : base(title, createChangedDocument, equivalenceKey)
        {
            SupportsFixAll = generateFixAll;
        }
    }

    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticDescriptors.Rule0051PossibleOverflowAssigning.Id);

    public sealed override FixAllProvider GetFixAllProvider() =>
         WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx)
    {
        Document document = ctx.Document;
        TextSpan span = ctx.Span;
        CancellationToken cancellationToken = ctx.CancellationToken;

        var syntaxRootTask = document.GetSyntaxRootAsync(cancellationToken);
        var semanticModelTask = document.GetSemanticModelAsync(cancellationToken);
        await Task.WhenAll(syntaxRootTask, semanticModelTask).ConfigureAwait(false);

        SyntaxNode? syntaxRoot = syntaxRootTask.Result;
        SemanticModel? semanticModel = semanticModelTask.Result;

        if (syntaxRoot is not null && semanticModel is not null)
        {
            RegisterInstanceCodeFix(ctx, syntaxRoot, semanticModel, span, document);
        }
    }

    private static void RegisterInstanceCodeFix(CodeFixContext ctx, SyntaxNode syntaxRoot, SemanticModel semanticModel, TextSpan span, Document document)
    {
        SyntaxNode node = syntaxRoot.FindNode(span);

        // Only register the fix if we can find a label variable declaration and get target length
        if (TryFindLabelDeclarationAndTargetLength(node, semanticModel, out var labelDeclaration, out var targetLength) && labelDeclaration is not null)
        {
            ctx.RegisterCodeFix(CreateCodeAction(labelDeclaration, document, targetLength, true), ctx.Diagnostics[0]);
        }
    }

    private static Fix0051AppendMaxLengthToLabelCodeAction CreateCodeAction(LabelDataTypeSyntax labelDeclaration, Document document,
        int targetLength, bool generateFixAll)
    {
        return new Fix0051AppendMaxLengthToLabelCodeAction(
            LinterCopAnalyzers.Fix0051AppendMaxLengthToLabelCodeAction,
            ct => AppendMaxLengthToLabel(document, labelDeclaration, targetLength, ct),
            nameof(Fix0051AppendMaxLengthToLabelCodeAction),
            generateFixAll);
    }

    private static bool TryFindLabelDeclarationAndTargetLength(SyntaxNode node, SemanticModel semanticModel, out LabelDataTypeSyntax? labelDeclaration, out int targetLength)
    {
        labelDeclaration = null;
        targetLength = 0;

        // The diagnostic is reported on the label identifier, so let's find the variable symbol
        if (semanticModel.GetSymbolInfo(node).Symbol is not IVariableSymbol variableSymbol)
            return false;

        // Get the variable declaration syntax
        if (variableSymbol.DeclaringSyntaxReference?.GetSyntax() is not VariableDeclarationSyntax variableSyntax)
            return false;

        // Check if it's a Label type
        if (variableSyntax.Type.DataType is not LabelDataTypeSyntax labelSyntax)
            return false;

        // Check if MaxLength property is already set - if so, don't offer the fix
        if (HasMaxLengthPropertySet(labelSyntax.Label.Properties))
            return false;

        // Get the target length from the context where the variable is used
        if (!TryExtractTargetLengthFromContext(node, semanticModel, out targetLength))
            return false;

        labelDeclaration = labelSyntax;
        return true;
    }

    private static bool TryExtractTargetLengthFromContext(SyntaxNode node, SemanticModel semanticModel, out int targetLength)
    {
        targetLength = 0;

        var parentNode = GetFirstParent(node,
            SyntaxKind.AssignmentStatement,
            SyntaxKind.ExitStatement,
            SyntaxKind.InvocationExpression);

        if (parentNode is null)
            return false;

        var operation = semanticModel.GetOperation(parentNode);
        if (operation is null)
            return false;

        var targetTypeSymbol = OperationHelper.TryGetTargetTypeSymbol(operation, semanticModel.Compilation);
        if (targetTypeSymbol == null)
            return false;

        bool isError = false;
        int fieldLength = targetTypeSymbol.GetTypeLength(ref isError);
        if (!isError || fieldLength > 0)
        {
            targetLength = fieldLength;
            return true;
        }

        return false;
    }

    private static SyntaxNode? GetFirstParent(SyntaxNode node, params SyntaxKind[] kinds)
    {
        var parent = node.Parent;
        while (parent is not null)
        {
            if (kinds.Contains(parent.Kind))
                return parent;

            parent = parent.Parent;
        }
        return null;
    }

    private static bool HasMaxLengthPropertySet(CommaSeparatedIdentifierEqualsLiteralListSyntax? properties)
    {
        if (properties is null)
            return false;

        foreach (var value in properties.Values)
        {
            var name = value.Identifier.Value?.ToString();
            if (!string.IsNullOrEmpty(name) && string.Equals(name, LabelPropertyHelper.MaxLength, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static async Task<Document> AppendMaxLengthToLabel(Document document, LabelDataTypeSyntax labelDeclaration, int targetLength, CancellationToken cancellationToken)
    {
        var maxLengthProperty = CreateMaxLengthProperty(targetLength);
        var propertyList = labelDeclaration.Label.Properties ?? SyntaxFactory.CommaSeparatedIdentifierEqualsLiteralList();
        propertyList = propertyList.AddValues(maxLengthProperty);

        // Replace the properties or the entire label declaration if no properties existed
        SyntaxNode newLabelDeclaration = labelDeclaration.Label.Properties is not null
            ? labelDeclaration.ReplaceNode(labelDeclaration.Label.Properties, propertyList)
            : ReplaceOrAddPropertiesInLabelDataType(labelDeclaration, propertyList);

        return await ReplaceNodeInDocument(document, labelDeclaration, newLabelDeclaration, cancellationToken)
            .ConfigureAwait(false);
    }

    private static LabelDataTypeSyntax ReplaceOrAddPropertiesInLabelDataType(LabelDataTypeSyntax original, CommaSeparatedIdentifierEqualsLiteralListSyntax propertyList)
    {
        // Create a new label with properties (and need to include an extra comma)
        var newLabel = SyntaxFactory.Label(original.Label.LabelText, SyntaxFactory.Token(SyntaxKind.CommaToken), propertyList);

        // Create a new label data type with the updated label
        var newLabelDataType = SyntaxFactory.LabelDataType(SyntaxFactory.Token(SyntaxKind.LabelKeyword), newLabel);

        // Find the original label keyword token and replace the new one with the original to preserve casing
        var originalKeywordToken = original.DescendantTokens().FirstOrDefault(t => t.IsKind(SyntaxKind.LabelKeyword));
        var newKeywordToken = newLabelDataType.DescendantTokens().FirstOrDefault(t => t.IsKind(SyntaxKind.LabelKeyword));

        if (!originalKeywordToken.IsKind(SyntaxKind.None) && !newKeywordToken.IsKind(SyntaxKind.None))
        {
            newLabelDataType = newLabelDataType.ReplaceToken(newKeywordToken, originalKeywordToken);
        }

        return newLabelDataType;
    }

    private static IdentifierEqualsLiteralSyntax CreateMaxLengthProperty(int maxLength)
    {
        var identifier = SyntaxFactory.Identifier(LabelPropertyHelper.MaxLength);
        var literalValue = SyntaxFactory.Int32SignedLiteralValue(SyntaxFactory.Literal(maxLength));

        return SyntaxFactory.IdentifierEqualsLiteral(identifier, literalValue);
    }

    private static async Task<Document> ReplaceNodeInDocument<T>(Document document, T oldNode, T newNode, CancellationToken cancellationToken)
      where T : SyntaxNode
    {
        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = syntaxRoot?.ReplaceNode(oldNode, newNode);
        return newRoot != null ? document.WithSyntaxRoot(newRoot) : document;
    }
}