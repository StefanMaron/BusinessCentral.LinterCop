namespace BusinessCentral.LinterCop.Test;

public class Rule0080
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0080");
    }

    [Test]
    [TestCase("SelectToken")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0080AnalyzeJsonTokenJPath>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0080AnalyzeJsonTokenJPath.Id);
    }

    [Test]
    [TestCase("SelectToken")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0080AnalyzeJsonTokenJPath>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0080AnalyzeJsonTokenJPath.Id);
    }
}