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
    [TestCase("SetFilterFieldCode")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0051SetFilterPossibleOverflow>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0051SetFilterPossibleOverflow.Id);
    }

    [Test]
#if !LessThenFall2023
    [TestCase("SetFilterFieldRef")]
#endif
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0051SetFilterPossibleOverflow>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0051SetFilterPossibleOverflow.Id);
    }
}