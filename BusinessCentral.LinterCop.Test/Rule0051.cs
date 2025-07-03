#if !LessThenFall2023RV1
namespace BusinessCentral.LinterCop.Test;

public class Rule0051
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0051");
    }

    [Test]
    [TestCase("AssignLabel")]
    [TestCase("ExitStatementLabel")]
#if !LessThenSpring2024
    [TestCase("GetMethodStringLiteral")]
    [TestCase("GetMethodStrSubstNo")]
#endif
    [TestCase("SetFilterFieldCode")]
    [TestCase("ValidateFieldCode")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0051PossibleOverflowAssigning>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0051PossibleOverflowAssigning.Id);
    }

    [Test]
    [TestCase("AssignLabel")]
    [TestCase("ExitStatementLabel")]
    [TestCase("ExitStatementLabelWithLocked")]
    [TestCase("ExitStatementLabelWithMaxLength")]
#if !LessThenSpring2024
    [TestCase("GetMethodCompanyName")]
    [TestCase("GetMethodStringLiteral")]
    [TestCase("GetMethodStrSubstNo")]
    [TestCase("GetMethodUserId")]
#endif
    [TestCase("SetFilterFieldRef")]
    [TestCase("ValidateFieldCode")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0051PossibleOverflowAssigning>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0051PossibleOverflowAssigning.Id);
    }
}
#endif