namespace BusinessCentral.LinterCop.Test;

public class Rule0076
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0076");
    }

    [Test]
    [TestCase("TableRelationLonger")]
#if !LessThenSpring2024
    [TestCase("TableExtRelationLonger")]
#endif
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0076TableRelationTooLong>();
        fixture.HasDiagnostic(code, Rule0076TableRelationTooLong.DiagnosticDescriptors.Rule0076TableRelationTooLong.Id);
    }

    [Test]
    [TestCase("TableRelationEqual")]
    [TestCase("TableRelationShorter")]
    [TestCase("TableExtRelationEqual")]
    [TestCase("TableExtRelationShorter")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0076TableRelationTooLong>();
        fixture.NoDiagnosticAtMarker(code, Rule0076TableRelationTooLong.DiagnosticDescriptors.Rule0076TableRelationTooLong.Id);
    }
}