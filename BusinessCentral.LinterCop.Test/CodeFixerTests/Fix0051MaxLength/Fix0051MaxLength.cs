using BusinessCentral.LinterCop.CodeFixes;
using BusinessCentral.LinterCop.Test.Helpers;

namespace BusinessCentral.LinterCop.Test.CodeFixerTests.Fix0051MaxLength;

internal class Fix0051MaxLength
{
    [Test]
    [TestCase("AssignmentStatement")]
    [TestCase("ExitStatement")]
    [TestCase("ExitStatementWithComment")]
    [TestCase("ValidateStatement")]
    public async Task HasFix(string testCase)
    {
        var currentCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0051MaxLength), nameof(HasFix), testCase, "current.al");
        var expectedCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0051MaxLength), nameof(HasFix), testCase, "expected.al");

        var fixture = RoslynFixtureFactory.Create<Fix0051AppendMaxLengthToLabelCodeFixProvider>(
            new CodeFixTestFixtureConfig
            {
                AdditionalAnalyzers = [new Rule0051PossibleOverflowAssigning()]
            });

        fixture.TestCodeFix(currentCode, expectedCode, DiagnosticDescriptors.Rule0051PossibleOverflowAssigning);
    }
}