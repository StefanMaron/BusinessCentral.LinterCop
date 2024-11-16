namespace BusinessCentral.LinterCop.Test;

public class Rule0044
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0044");
    }

#if !LessThenSpring2024
    [Test]
    [TestCase("1")]
    [TestCase("2")]
#endif
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0044AnalyzeTransferFields>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0044AnalyzeTransferFields.Id);
    }

#if !LessThenSpring2024
    [Test]
    [TestCase("1")]
    [TestCase("2")]
#endif
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0044AnalyzeTransferFields>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0044AnalyzeTransferFields.Id);
    }
}