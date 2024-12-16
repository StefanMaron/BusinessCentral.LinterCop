using System;
using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeFixes;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions.Mef;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;

namespace BusinessCentral.LinterCop.CodeFixes;

public class FixSomething : CodeFixProvider
{
    private class DoFixSomething : CodeAction.DocumentChangeAction
    {
        public override CodeActionKind Kind => CodeActionKind.QuickFix;

        public DoFixSomething(string title, Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey)
            : base(title, createChangedDocument, equivalenceKey)
        {
        }
    }

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable.Id);

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxFactory.Token(SyntaxKind.LocalKeyword).WithLeadingTrivia();

        return Task.FromResult(context.Document);
    }
}