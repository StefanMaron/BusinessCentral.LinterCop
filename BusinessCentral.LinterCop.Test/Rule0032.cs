namespace BusinessCentral.LinterCop.Test;

public class Rule0032
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0032");
    }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0032ClearCodeunitSingleInstance>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0032ClearCodeunitSingleInstance.Id);
    }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0032ClearCodeunitSingleInstance>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0032ClearCodeunitSingleInstance.Id);
    }
}
