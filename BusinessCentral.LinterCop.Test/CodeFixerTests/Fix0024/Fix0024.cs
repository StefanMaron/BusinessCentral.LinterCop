using BusinessCentral.LinterCop.CodeFixes;
using BusinessCentral.LinterCop.Test.Helpers;

namespace BusinessCentral.LinterCop.Test.CodeFixerTests.Fix0024;

internal class Fix0024
{
    [Test]
    [TestCase("Semicolon")]
    public async Task HasFix(string testCase)
    {
        var currentCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0024), nameof(HasFix), testCase, "current.al");
        var expectedCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0024), nameof(HasFix), testCase, "expected.al");

        var fixture = RoslynFixtureFactory.Create<Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeFixProvider>(
            new CodeFixTestFixtureConfig
            {
                AdditionalAnalyzers = [new Rule0024SemicolonAfterMethodOrTriggerDeclaration()]
            });

        fixture.TestCodeFix(currentCode, expectedCode, DiagnosticDescriptors.Rule0024SemicolonAfterMethodOrTriggerDeclaration);
    }
}