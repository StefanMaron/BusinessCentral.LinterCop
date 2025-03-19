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
    [TestCase("TableRelationImplicitFieldPrimaryKey")]
    [TestCase("TableRelationImplicitFieldPrimaryKeyWithNamespace")]
#if !LessThenSpring2024
    [TestCase("TableExtRelationLonger")]
#endif
    [TestCase("TableRelationWithNamespace")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0076TableRelationTooLong>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0076TableRelationTooLong.Id);
    }

    [Test]
    [TestCase("TableRelationEqual")]
    [TestCase("TableRelationShorter")]
#if !LessThenSpring2024
    [TestCase("TableExtRelationEqual")]
    [TestCase("TableExtRelationShorter")]
#endif
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0076TableRelationTooLong>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0076TableRelationTooLong.Id);
    }
}