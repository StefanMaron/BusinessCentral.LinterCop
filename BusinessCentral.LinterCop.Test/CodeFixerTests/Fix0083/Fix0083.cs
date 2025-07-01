using BusinessCentral.LinterCop.CodeFixes;
using BusinessCentral.LinterCop.Test.Helpers;

namespace BusinessCentral.LinterCop.Test.CodeFixerTests.Fix0083;

internal class Fix0083
{
    [Test]
    [TestCase("Date2DMY_Day")]
    [TestCase("Date2DMY_Month")]
    [TestCase("Date2DMY_Year")]
    [TestCase("Date2DWY_DayOfWeek")]
    [TestCase("Date2DWY_WeekNo")]
    [TestCase("DT2Date")]
    [TestCase("DT2Time")]
    [TestCase("Format_Hour")]
    [TestCase("Format_Minute")]
    [TestCase("Format_Second")]
    [TestCase("Format_Millisecond")]
    public async Task HasFix(string testCase)
    {
        var currentCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0083), nameof(HasFix), testCase, "current.al");
        var expectedCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0083), nameof(HasFix), testCase, "expected.al");

        var fixture = RoslynFixtureFactory.Create<Fix0083BuiltInDateTimeMethodCodeFixProvider>(
            new CodeFixTestFixtureConfig
            {
                AdditionalAnalyzers = [new Design.Rule0083BuiltInDateTimeMethod()]
            });

        fixture.TestCodeFix(currentCode, expectedCode, DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod);
    }
}
