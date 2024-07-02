namespace BusinessCentral.LinterCop.Test;

public class Rule0004
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0004");
    }

    [Test]
    [TestCase("1")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0004LookupPageIdAndDrillDownPageId>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId.Id);
    }

    [Test]
    [TestCase("1")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0004LookupPageIdAndDrillDownPageId>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0004LookupPageIdAndDrillDownPageId.Id);
    }
}