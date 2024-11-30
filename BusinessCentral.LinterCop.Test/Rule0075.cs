namespace BusinessCentral.LinterCop.Test;

public class Rule0075
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0075");
    }

    [Test]
    [TestCase("StyleExpr")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0075PageStyleDataType>();
        fixture.HasDiagnostic(code, Rule0075PageStyleDataType.DiagnosticDescriptors.Rule0075PageStyleDataType.Id);
    }

    [Test]
    [TestCase("StyleExpr")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0075PageStyleDataType>();
        fixture.NoDiagnosticAtMarker(code, Rule0075PageStyleDataType.DiagnosticDescriptors.Rule0075PageStyleDataType.Id);
    }
}