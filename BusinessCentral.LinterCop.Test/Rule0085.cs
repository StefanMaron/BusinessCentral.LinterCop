namespace BusinessCentral.LinterCop.Test;

public class Rule0085
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0085");
    }

    [Test]
    [TestCase("LFSeparatorChar")]
    [TestCase("LFSeparatorCode")]
    [TestCase("LFSeparatorText")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0085LFSeparator>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0085LFSeparator.Id);
    }

    [Test]
    [TestCase("LFSeparatorCodeElementAccess3")]
    [TestCase("LFSeparatorTextElementAccess3")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0085LFSeparator>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0085LFSeparator.Id);
    }
}