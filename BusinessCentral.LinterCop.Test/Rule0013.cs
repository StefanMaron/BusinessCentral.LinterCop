namespace BusinessCentral.LinterCop.Test;

public class Rule0013
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0013");
    }

    [Test]
    [TestCase("PrimaryKeyCodeField")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys.Id);
    }

    [Test]
    [TestCase("PrimaryKeyCodeFieldNotBlankFalse")]
    [TestCase("PrimaryKeyCodeFieldNotBlankTrue")]
    [TestCase("PrimaryKeyIntegerField")]
    [TestCase("PrimaryKeyMultipleFields")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys.Id);
    }
}