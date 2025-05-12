using BusinessCentral.LinterCop.CodeFixes;
using BusinessCentral.LinterCop.Test.Helpers;

namespace BusinessCentral.LinterCop.Test.CodeFixerTests.Fix0077;

internal class Fix0077
{
    [Test]
    [TestCase("CurrentDateTime")]
    [TestCase("CompanyName")]
    [TestCase("TableCaption")]
    public async Task HasFix(string testCase)
    {
        var currentCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0077), nameof(HasFix), testCase, "current.al");
        var expectedCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0077), nameof(HasFix), testCase, "expected.al");

        var fixture = RoslynFixtureFactory.Create<Fix0077UseParenthesisForFunctionCallCodeFixProvider>(
            new CodeFixTestFixtureConfig
            {
                AdditionalAnalyzers = [new Rule0077UseParenthesisForFunctionCall()]
            });

        fixture.TestCodeFix(currentCode, expectedCode, DiagnosticDescriptors.Rule0077UseParenthesisForFunctionCall);
    }
}