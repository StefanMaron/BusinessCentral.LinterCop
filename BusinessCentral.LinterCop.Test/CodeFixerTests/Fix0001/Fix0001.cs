using BusinessCentral.LinterCop.CodeFixes;
using BusinessCentral.LinterCop.Test.Helpers;

namespace BusinessCentral.LinterCop.Test.CodeFixerTests.Fix0001;

internal class Fix0001
{
    [Test]
    [TestCase("SingleFlowFieldIsEditable")]
    public async Task HasFix(string testCase)
    {
        var currentCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0001), nameof(HasFix), testCase, "current.al");
        var expectedCode = await CodeFixerTestHelpers.GetCodeFixerTestCode(nameof(Fix0001), nameof(HasFix), testCase, "expected.al");

        var fixture = RoslynFixtureFactory.Create<Fix0001FlowFieldsShouldNotBeEditableCodeFixProvider>(
            new CodeFixTestFixtureConfig
            {
                AdditionalAnalyzers = [new Rule0001FlowFieldsShouldNotBeEditable()]
            });

        fixture.TestCodeFix(currentCode, expectedCode, DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable);
    }

}