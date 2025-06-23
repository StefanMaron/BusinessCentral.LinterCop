namespace BusinessCentral.LinterCop.Test;

public class Rule0023
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0023");
    }

    [Test]
    [TestCase("BrickIsMissing")]
    [TestCase("DropDownIsMissing")]
    [TestCase("TemporaryTable")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0023AlwaysSpecifyFieldgroups>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups.Id);
    }

    [Test]
    [TestCase("HasBrickAndDropDown")]
    [TestCase("TemporaryTable")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0023AlwaysSpecifyFieldgroups>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups.Id);
    }
}