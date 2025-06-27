using BusinessCentral.LinterCop.CodeFixes;
using BusinessCentral.LinterCop.Test.Helpers;

namespace BusinessCentral.LinterCop.Test.CodeFixerTests.Fix0051ApplyCopyStr;

// Somehow AL version 12.0 throws an error 
// error CS0246: The type or namespace name 'Rule0051PossibleOverflowAssigning' could not be found (are you missing a using directive or an assembly reference?) 
#if !LessThenFall2023RV1
internal class Fix0051ApplyCopyStr
{
    [Test]
    [TestCase("AssignmentStatement")]
    [TestCase("ExitStatement")]
    [TestCase("ValidateStatement")]
    public async Task HasFix(string testCase)
    {
        var currentCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0051ApplyCopyStr), nameof(HasFix), testCase, "current.al");
        var expectedCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0051ApplyCopyStr), nameof(HasFix), testCase, "expected.al");

        var fixture = RoslynFixtureFactory.Create<Fix0051ApplyCopyStrCodeFixProvider>(
            new CodeFixTestFixtureConfig
            {
                AdditionalAnalyzers = [new Rule0051PossibleOverflowAssigning()]
            });

        fixture.TestCodeFix(currentCode, expectedCode, DiagnosticDescriptors.Rule0019DataClassificationFieldEqualsTable);
    }
}
#endif