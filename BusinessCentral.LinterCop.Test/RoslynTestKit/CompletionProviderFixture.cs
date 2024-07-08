using System.Collections.Immutable;
using BusinessCentral.LinterCop.Test.RoslynTestKit.Utils;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces.Completion;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public abstract class CompletionProviderFixture : BaseTestFixture
{
    public void TestCompletion(string markupCode, string[] expectedCompletions, CompletionTrigger? trigger = null)
    {
        var markup = new CodeMarkup(markupCode);
        var document = CreateDocumentFromCode(markup.Code);
        var assertion = CreateAssertionBasedOnExpectedSet(expectedCompletions, markup.Locator);
        VerifyExpectations(document, markup.Locator, trigger, assertion);
    }

    public void TestCompletion(string markupCode, Action<ImmutableArray<CompletionItem>> assertion,
        CompletionTrigger? trigger = null)
    {
        var markup = new CodeMarkup(markupCode);
        var document = CreateDocumentFromCode(markup.Code);
        VerifyExpectations(document, markup.Locator, trigger, assertion);
    }

    public void TestCompletion(Document document, TextSpan span, string[] expectedCompletions,
        CompletionTrigger? trigger = null)
    {
        var locator = new TextSpanLocator(span);
        var assertion = CreateAssertionBasedOnExpectedSet(expectedCompletions, locator);
        VerifyExpectations(document, locator, trigger, assertion);
    }

    public void TestCompletion(Document document, TextSpan span, Action<ImmutableArray<CompletionItem>> assertion,
        CompletionTrigger? trigger = null)
    {
        var locator = new TextSpanLocator(span);
        VerifyExpectations(document, locator, trigger, assertion);
    }

    private static Action<ImmutableArray<CompletionItem>> CreateAssertionBasedOnExpectedSet(
        string[] expectedCompletions, IDiagnosticLocator locator)
    {
        return items =>
        {
            var allFoundCompletionText = items.Select(x => x.DisplayText);
            var missingSuggestions = expectedCompletions.Except(allFoundCompletionText).ToList();

            if (missingSuggestions.Count > 0)
            {
                throw RoslynTestKitException.CannotFindSuggestion(missingSuggestions, items, locator);
            }
        };
    }

    private void VerifyExpectations(Document document, IDiagnosticLocator locator, CompletionTrigger? trigger,
        Action<ImmutableArray<CompletionItem>> assertion)
    {
        var selectedTrigger = trigger ?? CompletionTrigger.Default;
        var provider = CreateProvider();
        var span = locator.GetSpan();
        Workspace.TryGetWorkspace(document.GetTextAsync().Result.Container, out var workspace);
        var service = new TestCompletionService(document.Project.Solution.Workspace, provider);
        var result = service.GetCompletionsAsync(document, span.Start, selectedTrigger, ImmutableHashSet<string>.Empty,
            workspace.Options, CancellationToken.None).GetAwaiter().GetResult();
        assertion(result?.Items ?? ImmutableArray<CompletionItem>.Empty);
    }

    protected abstract CompletionProvider CreateProvider();
}