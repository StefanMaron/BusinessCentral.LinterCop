namespace BusinessCentral.LinterCop.Test;

public class Rule0093
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0093");
    }

    [Test]
    [TestCase("GlobalTestMethodWithoutTestAttribute")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0093GlobalTestMethodRequiresTestAttribute>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0093GlobalTestMethodRequiresTestAttribute.Id);
    }

    [Test]
    [TestCase("StandardCodeunit")]
    [TestCase("ConfirmHandler")]
    [TestCase("GlobalTestProcedure")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0093GlobalTestMethodRequiresTestAttribute>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0093GlobalTestMethodRequiresTestAttribute.Id);
    }
}