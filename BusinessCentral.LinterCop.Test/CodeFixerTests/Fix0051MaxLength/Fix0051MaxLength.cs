using BusinessCentral.LinterCop.CodeFixes;
using BusinessCentral.LinterCop.Test.Helpers;

namespace BusinessCentral.LinterCop.Test.CodeFixerTests.Fix0051MaxLength;

// Somehow AL version 12.0 throws an error 
// Fix0051.cs(20,44): error CS0246: The type or namespace name 'Rule0051PossibleOverflowAssigning' could not be found (are you missing a using directive or an assembly reference?) 
#if !LessThenFall2023
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
#endif