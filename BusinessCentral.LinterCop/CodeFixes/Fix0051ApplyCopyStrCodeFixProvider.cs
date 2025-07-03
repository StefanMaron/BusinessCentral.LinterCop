using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef;

namespace BusinessCentral.LinterCop.CodeFixes;

[CodeFixProvider(nameof(Fix0051ApplyCopyStrCodeFixProvider))]
public sealed class Fix0051ApplyCopyStrCodeFixProvider : CodeFixProvider
{
    private const string TextClassName = "Text";
    private const string CopyStrMethodName = "CopyStr";
    private const string MaxStrLenMethodName = "MaxStrLen";
    private const int StartPositionForCopyStr = 1;

    private class Fix0051ApplyCopyStrCodeAction : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;
        public override bool SupportsFixAll { get; }
        public override string? FixAllSingleInstanceTitle => string.Empty;
        public override string? FixAllTitle => Title;

        public Fix0051ApplyCopyStrCodeAction(string title,
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

        SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        RegisterInstanceCodeFix(ctx, syntaxRoot, span, document);
    }

    private static void RegisterInstanceCodeFix(CodeFixContext ctx, SyntaxNode syntaxRoot, TextSpan span, Document document)
    {
        SyntaxNode node = syntaxRoot.FindNode(span);
        ctx.RegisterCodeFix(CreateCodeAction(node, document, true), ctx.Diagnostics[0]);
    }

    private static Fix0051ApplyCopyStrCodeAction CreateCodeAction(SyntaxNode node, Document document,
        bool generateFixAll)
    {
        return new Fix0051ApplyCopyStrCodeAction(
            LinterCopAnalyzers.Fix0051ApplyCopyStrCodeAction,
            ct => ApplyCopyStr(document, node, ct),
            nameof(Fix0051ApplyCopyStrCodeAction),
            generateFixAll);
    }

    private static async Task<Document> ApplyCopyStr(Document document, SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node is not ExpressionSyntax sourceExpression)
            return document;

        if (node.GetFirstParent(SyntaxKind.AssignmentStatement) is AssignmentStatementSyntax assignmentStatement)
            return await HandleAssignmentStatement(document, sourceExpression, assignmentStatement, cancellationToken);

        if (node.GetFirstParent(SyntaxKind.ExitStatement) is ExitStatementSyntax exitStatementSyntax)
            return await HandleExitStatement(document, sourceExpression, exitStatementSyntax, cancellationToken);

        if (node.GetFirstParent(SyntaxKind.InvocationExpression) is InvocationExpressionSyntax invocationExpression)
            return await HandleValidateInvocation(document, sourceExpression, invocationExpression, cancellationToken);

        return document;
    }

    private static async Task<Document> HandleAssignmentStatement(Document document, ExpressionSyntax sourceExpression,
        AssignmentStatementSyntax assignmentStatement, CancellationToken cancellationToken)
    {
        // Get the target variable name (left side of assignment)
        if (assignmentStatement.Target is not { } targetExpression)
            return document;

        // Create Text.CopyStr(source, 1, Text.MaxStrLen(target)) expression
        var copyStrExpression = CreateCopyStrExpressionWithMaxStrLen(sourceExpression, targetExpression);

        // Replace the source expression with the CopyStr call in the assignment
        var newAssignmentStatement = assignmentStatement.ReplaceNode(sourceExpression, copyStrExpression);

        return await ReplaceNodeInDocument(document, assignmentStatement, newAssignmentStatement, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> HandleExitStatement(Document document, ExpressionSyntax sourceExpression,
        ExitStatementSyntax exitStatement, CancellationToken cancellationToken)
    {
        // Get the method declaration to extract return type length
        if (exitStatement.GetFirstParent(SyntaxKind.MethodDeclaration) is not MethodDeclarationSyntax methodDeclaration ||
            methodDeclaration.ReturnValue?.DataType.DataType is not LengthDataTypeSyntax lengthDataType ||
            lengthDataType.Length.Value is not int returnTypeLength)
        {
            return document;
        }

        // Create Text.CopyStr(source, 1, returnTypeLength) expression
        var copyStrExpression = CreateCopyStrExpressionWithLength(sourceExpression, returnTypeLength);

        // Replace the source expression with the CopyStr call in the exit statement
        var newExitStatement = exitStatement.ReplaceNode(sourceExpression, copyStrExpression);

        return await ReplaceNodeInDocument(document, exitStatement, newExitStatement, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> HandleValidateInvocation(Document document, ExpressionSyntax sourceExpression,
        InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        // Check if this is a Validate method call with at least 2 arguments
        if (invocationExpression.ArgumentList?.Arguments.Count < 2)
            return document;

        var targetFieldExpression = GetTargetFieldExpression(invocationExpression);
        if (targetFieldExpression == null)
            return document;

        // Create Text.CopyStr(source, 1, Text.MaxStrLen(targetFieldExpression)) expression
        var copyStrExpression = CreateCopyStrExpressionWithMaxStrLen(sourceExpression, targetFieldExpression);

        // Replace the source expression with the CopyStr call in the invocation
        var newInvocationExpression = invocationExpression.ReplaceNode(sourceExpression, copyStrExpression);

        return await ReplaceNodeInDocument(document, invocationExpression, newInvocationExpression, cancellationToken).ConfigureAwait(false);
    }

    private static ExpressionSyntax? GetTargetFieldExpression(InvocationExpressionSyntax invocationExpression)
    {
        var firstArgument = invocationExpression.ArgumentList?.Arguments[0];
        if (firstArgument is not CodeExpressionSyntax fieldExpression)
            return null;

        // Handle MyTable.Validate(MyField, ...) case
        if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var tableExpression = memberAccess.Expression;
            // Create MyTable.MyField expression for MaxStrLen
            return SyntaxFactory.MemberAccessExpression(tableExpression, (IdentifierNameSyntax)fieldExpression);
        }

        // Fallback: use the first argument as is
        return fieldExpression;
    }

    private static ExpressionSyntax CreateCopyStrExpressionWithMaxStrLen(ExpressionSyntax sourceExpression, ExpressionSyntax targetExpression)
    {
        // Create: Text.CopyStr(sourceExpression, 1, Text.MaxStrLen(targetExpression))
        var maxStrLenExpression = CreateMaxStrLenExpression(targetExpression);
        return CreateCopyStrExpression(sourceExpression, maxStrLenExpression);
    }

    private static ExpressionSyntax CreateCopyStrExpressionWithLength(ExpressionSyntax sourceExpression, int length)
    {
        // Create: Text.CopyStr(sourceExpression, 1, length)
        var lengthLiteral = CreateIntegerLiteral(length);
        return CreateCopyStrExpression(sourceExpression, lengthLiteral);
    }

    private static ExpressionSyntax CreateCopyStrExpression(ExpressionSyntax sourceExpression, ExpressionSyntax lengthExpression)
    {
        // Create: Text.CopyStr(sourceExpression, 1, lengthExpression)
        var textIdentifier = CreateTextIdentifier();
        var copyStrMemberAccess = SyntaxFactory.MemberAccessExpression(textIdentifier, CreateCopyStringIdentifier());
        var startPositionLiteral = CreateIntegerLiteral(StartPositionForCopyStr);

        var copyStrArguments = default(SeparatedSyntaxList<CodeExpressionSyntax>)
            .Add((CodeExpressionSyntax)sourceExpression)
            .Add(startPositionLiteral)
            .Add((CodeExpressionSyntax)lengthExpression);
        var copyStrArgumentList = SyntaxFactory.ArgumentList(copyStrArguments);
        var copyStrInvocation = SyntaxFactory.InvocationExpression(copyStrMemberAccess, copyStrArgumentList);

        return copyStrInvocation.WithTriviaFrom(sourceExpression);
    }

    private static InvocationExpressionSyntax CreateMaxStrLenExpression(ExpressionSyntax targetExpression)
    {
        // Create: Text.MaxStrLen(targetExpression)
        var textIdentifier = CreateTextIdentifier();
        var maxStrLenMemberAccess = SyntaxFactory.MemberAccessExpression(textIdentifier, CreateMaxStringLengthIdentifier());
        var maxStrLenArguments = default(SeparatedSyntaxList<CodeExpressionSyntax>).Add((CodeExpressionSyntax)targetExpression);
        var maxStrLenArgumentList = SyntaxFactory.ArgumentList(maxStrLenArguments);
        return SyntaxFactory.InvocationExpression(maxStrLenMemberAccess, maxStrLenArgumentList);
    }

    private static IdentifierNameSyntax CreateCopyStringIdentifier()
        => SyntaxFactory.IdentifierName(CopyStrMethodName);

    private static IdentifierNameSyntax CreateTextIdentifier()
        => SyntaxFactory.IdentifierName(TextClassName);

    private static IdentifierNameSyntax CreateMaxStringLengthIdentifier()
            => SyntaxFactory.IdentifierName(MaxStrLenMethodName);

    private static LiteralExpressionSyntax CreateIntegerLiteral(int value)
    {
        return SyntaxFactory.LiteralExpression(SyntaxFactory.Int32SignedLiteralValue(SyntaxFactory.Literal(value)));
    }

    private static async Task<Document> ReplaceNodeInDocument<T>(Document document, T oldNode, T newNode, CancellationToken cancellationToken)
      where T : SyntaxNode
    {
        var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = syntaxRoot?.ReplaceNode(oldNode, newNode);
        return newRoot != null ? document.WithSyntaxRoot(newRoot) : document;
    }
}