namespace BusinessCentral.LinterCop.Test;

public class Rule0067
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0067");
    }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    [TestCase("3")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys>();
        fixture.HasDiagnostic(code, Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys.DiagnosticDescriptors.Rule0067DisableNotBlankOnSingleFieldPrimaryKey.Id);
    }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    [TestCase("3")]
    [TestCase("4")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys>();
        fixture.NoDiagnosticAtMarker(code, Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys.DiagnosticDescriptors.Rule0067DisableNotBlankOnSingleFieldPrimaryKey.Id);
    }
}