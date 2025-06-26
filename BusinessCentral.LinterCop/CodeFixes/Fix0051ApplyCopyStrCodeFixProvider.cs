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
    private class Fix0051ApplyCopStrCodeAction : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;
        public override bool SupportsFixAll { get; }
        public override string? FixAllSingleInstanceTitle => string.Empty;
        public override string? FixAllTitle => Title;

        public Fix0051ApplyCopStrCodeAction(string title,
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

    private static Fix0051ApplyCopStrCodeAction CreateCodeAction(SyntaxNode node, Document document,
        bool generateFixAll)
    {
        return new Fix0051ApplyCopStrCodeAction(
            LinterCopAnalyzers.Fix0051ApplyCopyStrCodeAction,
            ct => ApplyCopyStr(document, node, ct),
            nameof(Fix0051ApplyCopStrCodeAction),
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
        var targetExpression = assignmentStatement.Target;
        if (targetExpression == null)
            return document;

        // Create Text.CopyStr(source, 1, Text.MaxStrLen(target)) expression
        var copyStrExpression = CreateCopyStrExpressionWithMaxStrLen(sourceExpression, targetExpression);

        // Replace the source expression with the CopyStr call in the assignment
        var newAssignmentStatement = assignmentStatement.ReplaceNode(sourceExpression, copyStrExpression);

        var newRoot = (await document.GetSyntaxRootAsync(cancellationToken)).ReplaceNode(assignmentStatement, newAssignmentStatement);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> HandleExitStatement(Document document, ExpressionSyntax sourceExpression,
        ExitStatementSyntax exitStatement, CancellationToken cancellationToken)
    {
        // Get the method declaration to extract return type length
        var methodDeclaration = exitStatement.GetFirstParent(SyntaxKind.MethodDeclaration) as MethodDeclarationSyntax;
        if (methodDeclaration?.ReturnValue?.DataType.DataType is not LengthDataTypeSyntax lengthDataType)
            return document;

        // Extract the length from the return type (e.g., Text[10] -> 10)
        int returnTypeLength = 0;
        if (lengthDataType.Length.Value is int length)
            returnTypeLength = length;

        // Create Text.CopyStr(source, 1, returnTypeLength) expression
        var copyStrExpression = CreateCopyStrExpressionWithLength(sourceExpression, returnTypeLength);

        // Replace the source expression with the CopyStr call in the exit statement
        var newExitStatement = exitStatement.ReplaceNode(sourceExpression, copyStrExpression);

        var newRoot = (await document.GetSyntaxRootAsync(cancellationToken)).ReplaceNode(exitStatement, newExitStatement);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> HandleValidateInvocation(Document document, ExpressionSyntax sourceExpression,
        InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken)
    {
        // Check if this is a Validate method call with at least 2 arguments
        if (invocationExpression.ArgumentList == null || invocationExpression.ArgumentList.Arguments.Count < 2)
            return document;

        // Get the table reference and field name from the invocation
        // For MyTable.Validate(MyField, value), we need to construct MyTable.MyField for MaxStrLen
        ExpressionSyntax targetFieldExpression;

        if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            // This is a case like MyTable.Validate(MyField, ...)
            var tableExpression = memberAccess.Expression;
            var firstArgument = invocationExpression.ArgumentList.Arguments[0];

            if (firstArgument is CodeExpressionSyntax fieldExpression)
            {
                // Create MyTable.MyField expression for MaxStrLen
                targetFieldExpression = SyntaxFactory.MemberAccessExpression(tableExpression, (IdentifierNameSyntax)fieldExpression);
            }
            else
            {
                return document;
            }
        }
        else
        {
            // Fallback: use the first argument as is
            var firstArgument = invocationExpression.ArgumentList.Arguments[0];
            if (firstArgument is not CodeExpressionSyntax fieldExpression)
                return document;

            targetFieldExpression = fieldExpression;
        }

        // Create Text.CopyStr(source, 1, Text.MaxStrLen(targetFieldExpression)) expression
        var copyStrExpression = CreateCopyStrExpressionWithMaxStrLen(sourceExpression, targetFieldExpression);

        // Replace the source expression with the CopyStr call in the invocation
        var newInvocationExpression = invocationExpression.ReplaceNode(sourceExpression, copyStrExpression);

        var newRoot = (await document.GetSyntaxRootAsync(cancellationToken)).ReplaceNode(invocationExpression, newInvocationExpression);
        return document.WithSyntaxRoot(newRoot);
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
        var oneLiteral = CreateIntegerLiteral(1);

        var copyStrArguments = default(SeparatedSyntaxList<CodeExpressionSyntax>)
            .Add((CodeExpressionSyntax)sourceExpression)
            .Add(oneLiteral)
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
    {
        return SyntaxFactory.IdentifierName("CopyStr");
    }
    private static IdentifierNameSyntax CreateTextIdentifier()
    {
        return SyntaxFactory.IdentifierName("Text");
    }

    private static IdentifierNameSyntax CreateMaxStringLengthIdentifier()
    {
        return SyntaxFactory.IdentifierName("MaxStrLen");
    }

    private static LiteralExpressionSyntax CreateIntegerLiteral(int value)
    {
        return SyntaxFactory.LiteralExpression(SyntaxFactory.Int32SignedLiteralValue(SyntaxFactory.Literal(value)));
    }
}