namespace BusinessCentral.LinterCop.Test;

public class Rule0086
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0086");
    }

    [Test]
    [TestCase("StyleExpr")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0086PageStyleDataType>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0086PageStyleDataType.Id);
    }

    [Test]
    [TestCase("Caption")]
    [TestCase("StyleExpr")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0086PageStyleDataType>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0086PageStyleDataType.Id);
    }
}