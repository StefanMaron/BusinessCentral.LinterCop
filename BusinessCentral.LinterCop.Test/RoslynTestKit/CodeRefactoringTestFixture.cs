using System.Collections.Immutable;
using BusinessCentral.LinterCop.Test.RoslynTestKit.CodeActionLocators;
using BusinessCentral.LinterCop.Test.RoslynTestKit.Utils;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeRefactoring;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public abstract class CodeRefactoringTestFixture : BaseTestFixture
{
    protected abstract CodeRefactoringProvider CreateProvider();

    protected virtual bool FailWhenInputContainsErrors => true;

    public void TestCodeRefactoring(string markupCode, string expected, ICodeActionSelector actionSelector)
    {
        var markup = new CodeMarkup(markupCode);
        var document = CreateDocumentFromCode(markup.Code);
        TestCodeRefactoring(document, expected, markup.Locator, actionSelector);
    }

    public void TestCodeRefactoring(string markupCode, string expected, int refactoringIndex = 0)
    {
        var markup = new CodeMarkup(markupCode);
        var document = CreateDocumentFromCode(markup.Code);
        TestCodeRefactoring(document, expected, markup.Locator, new ByIndexCodeActionSelector(refactoringIndex));
    }

    public void TestCodeRefactoring(string markupCode, string expected, string title)
    {
        var markup = new CodeMarkup(markupCode);
        var document = CreateDocumentFromCode(markup.Code);
        TestCodeRefactoring(document, expected, markup.Locator, new ByTitleCodeActionSelector(title));
    }

    public void TestCodeRefactoringAtLine(string code, string expected, int line, int refactoringIndex = 0)
    {
        var document = CreateDocumentFromCode(code);
        var locator = LineLocator.FromCode(code, line);
        TestCodeRefactoring(document, expected, locator, new ByIndexCodeActionSelector(refactoringIndex));
    }

    public void TestCodeRefactoringAtLine(Document document, string expected, int line, int refactoringIndex = 0)
    {
        var locator = LineLocator.FromDocument(document, line);
        TestCodeRefactoring(document, expected, locator, new ByIndexCodeActionSelector(refactoringIndex));
    }

    public void TestCodeRefactoring(Document document, string expected, TextSpan span, int refactoringIndex = 0)
    {
        var locator = new TextSpanLocator(span);
        TestCodeRefactoring(document, expected, locator, new ByIndexCodeActionSelector(refactoringIndex));
    }

    private void TestCodeRefactoring(Document document, string expected, IDiagnosticLocator locator,
        ICodeActionSelector codeActionSelector)
    {
        //if (FailWhenInputContainsErrors)
        //{
        //    var errors = document.GetErrors();
        //    if (errors.Count > 0)
        //    {
        //        throw RoslynTestKitException.UnexpectedErrorDiagnostic(errors);
        //    }
        //}

        var codeRefactorings = GetCodeRefactorings(document, locator);
        var selectedRefactoring = codeActionSelector.Find(codeRefactorings);

        if (selectedRefactoring is null)
        {
            throw RoslynTestKitException.CodeRefactoringNotFound(codeActionSelector, codeRefactorings, locator);
        }

        Verify.CodeAction(selectedRefactoring, document, expected);
    }

    protected void TestNoCodeRefactoring(string markupCode)
    {
        var markup = new CodeMarkup(markupCode);
        var document = CreateDocumentFromCode(markup.Code);
        TestNoCodeRefactoring(document, markup.Locator);
    }

    protected void TestNoCodeRefactoring(Document document, TextSpan span)
    {
        var locator = new TextSpanLocator(span);
        TestNoCodeRefactoring(document, locator);
    }

    private void TestNoCodeRefactoring(Document document, IDiagnosticLocator locator)
    {
        var codeRefactorings = GetCodeRefactorings(document, locator).ToImmutableArray();
        if (codeRefactorings.Length != 0)
        {
            throw RoslynTestKitException.UnexpectedCodeRefactorings(codeRefactorings);
        }
    }

    private ImmutableArray<CodeAction> GetCodeRefactorings(Document document, IDiagnosticLocator locator)
    {
        var builder = ImmutableArray.CreateBuilder<CodeAction>();
        var context =
            new CodeRefactoringContext(document, locator.GetSpan(), a => builder.Add(a), CancellationToken.None);
        var provider = CreateProvider();
        provider.ComputeRefactoringsAsync(context).Wait();
        return builder.ToImmutable();
    }
}