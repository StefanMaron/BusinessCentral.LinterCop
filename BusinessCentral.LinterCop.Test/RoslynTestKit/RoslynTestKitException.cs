using System.Collections.Immutable;
using BusinessCentral.LinterCop.Test.RoslynTestKit.CodeActionLocators;
using BusinessCentral.LinterCop.Test.RoslynTestKit.Utils;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces.Completion;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public class RoslynTestKitException : Exception
{
    public RoslynTestKitException(string message) : base(message)
    {
    }

    public static RoslynTestKitException UnexpectedDiagnostic(List<Diagnostic> unexpectedDiagnostics)
    {
        var messages = unexpectedDiagnostics.Select(d =>
        {
            var position = d.Location.GetLineSpan().StartLinePosition;
            return
                $"Found reported diagnostic '{d.Id}' in spite of the expectations at Line:{position.Line} Col:{position.Character}. Details: {d.GetMessage()}";
        });
        var description = string.Join("\r\n", messages);
        return new RoslynTestKitException(description);
    }

    public static Exception UnexpectedErrorDiagnostic(IReadOnlyList<Diagnostic> errors)
    {
        var messages = errors.MergeWithNewLines(x => x.ToString());
        return new RoslynTestKitException($"Input document contains errors: \r\n {messages}");
    }

    internal static Exception ExceptionInAnalyzer(
        Diagnostic diagnostic)
    {
        var message = diagnostic.Descriptor.Description.ToString();
        return new RoslynTestKitException(message);
    }

    public static RoslynTestKitException DiagnosticNotFound(string diagnosticId, IDiagnosticLocator locator,
        Diagnostic[] reportedDiagnostics)
    {
        var reportedDiagnosticInfo = reportedDiagnostics.MergeWithComma(x => x.Id, title: "Reported issues: ");
        var message =
            $"There is no issue reported for {diagnosticId} at {locator.Description()}.{reportedDiagnosticInfo}";
        return new RoslynTestKitException(message);
    }

    public static RoslynTestKitException CodeFixNotFound(ICodeActionSelector codeActionSelector,
        ImmutableArray<CodeAction> codeFixes, IDiagnosticLocator locator)
    {
        var codeFixDescription = GetActionsDescription(codeFixes, $" Found only {codeFixes.Length} CodeFixes: ");
        var message = $"Cannot find CodeFix {codeActionSelector} at {locator.Description()}.{codeFixDescription}";
        return new RoslynTestKitException(message);
    }

    public static RoslynTestKitException UnexpectedCodeFixFound(ImmutableArray<CodeAction> codeFixes,
        IDiagnosticLocator locator)
    {
        var codeFixDescription = GetActionsDescription(codeFixes, "Reported fixes: ");
        var message = $"Found unexpected CodeFixes at {locator.Description()}.{codeFixDescription}";
        return new RoslynTestKitException(message);
    }

    public static RoslynTestKitException CodeRefactoringNotFound(ICodeActionSelector codeActionSelector,
        ImmutableArray<CodeAction> codeRefactorings, IDiagnosticLocator locator)
    {
        var refactoringDescriptions =
            GetActionsDescription(codeRefactorings, $" Found only {codeRefactorings.Length} CodeRefactorings: ");
        var message =
            $"Cannot find CodeRefactoring {codeActionSelector}  at {locator.Description()}.{refactoringDescriptions}";
        return new RoslynTestKitException(message);
    }

    public static RoslynTestKitException UnexpectedCodeRefactorings(ImmutableArray<CodeAction> codeRefactorings)
    {
        var refactoringDescriptions = GetActionsDescription(codeRefactorings);
        return new RoslynTestKitException(
            $"Found reported CodeRefactorings '{refactoringDescriptions}' in spite of the expectations ");
    }

    private static string GetActionsDescription(ImmutableArray<CodeAction> codeFixes, string title = null)
    {
        if (codeFixes.Length == 0)
        {
            return string.Empty;
        }

        return title + Environment.NewLine + string.Join(Environment.NewLine, codeFixes.SelectMany((x, index) =>
        {
            if (NestedCodeActionHelper.TryGetNestedAction(x) is { } nestedAction)
            {
                return nestedAction.Select(n => $"[{index}] = {x.Title} -> {n.Title}").ToArray();
            }

            return new[] { $"[{index}] = {x.Title}" };
        })) + Environment.NewLine;
    }

    public static RoslynTestKitException NoOperationForCodeAction(CodeAction codeAction)
    {
        return new RoslynTestKitException($"There is no operation associated with '{codeAction.Title}'");
    }

    public static RoslynTestKitException MoreThanOneOperationForCodeAction(CodeAction codeAction,
        List<CodeActionOperation> operations)
    {
        var foundOperationDescriptions = operations.MergeWithComma(x => x.Title, title: " Found operations: ");
        return new RoslynTestKitException(
            $"There is more than one operation associated with '{codeAction.Title}'.{foundOperationDescriptions}");
    }

    public static Exception CannotFindSuggestion(IReadOnlyList<string> missingCompletion,
        ImmutableArray<CompletionItem> resultItems, IDiagnosticLocator locator)
    {
        var foundSuggestionDescription =
            resultItems.MergeAsBulletList(x => x.DisplayText, title: "\r\nFound suggestions:\r\n");
        return new RoslynTestKitException(
            $"Cannot get suggestions:\r\n{missingCompletion.MergeAsBulletList()}\r\nat{locator.Description()}{foundSuggestionDescription}");
    }
}