namespace BusinessCentral.LinterCop.Test;

public class Rule0008
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0008");
    }

    [Test]
    [TestCase("1")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0008NoFilterOperatorsInSetRange>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0008NoFilterOperatorsInSetRange.Id);
    }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0008NoFilterOperatorsInSetRange>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0008NoFilterOperatorsInSetRange.Id);
    }
}