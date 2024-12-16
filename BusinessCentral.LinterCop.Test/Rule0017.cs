namespace BusinessCentral.LinterCop.Test;

public class Rule0017
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0017");
    }

    [Test]
    [TestCase("Assignment")]
    [TestCase("Validate")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0017WriteToFlowField>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0017WriteToFlowField.Id);
    }

    [Test]
    [TestCase("Assignment")]
    [TestCase("Validate")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0017WriteToFlowField>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0017WriteToFlowField.Id);
    }
}