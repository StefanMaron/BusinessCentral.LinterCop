namespace BusinessCentral.LinterCop.Test;

public class Rule0078
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0078");
    }

    [Test]
    [TestCase("TempVar")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0078TemporaryRecordsShouldNotTriggerTableTriggers>();
        fixture.HasDiagnostic(code, Rule0078TemporaryRecordsShouldNotTriggerTableTriggers.DiagnosticDescriptors.Rule0078TemporaryRecordsShouldNotTriggerTableTriggers.Id);
    }

    [Test]
    [TestCase("TempVarImplicit")]
    [TestCase("TempVarExplicit")]
    [TestCase("TempTable")]
    [TestCase("TempTableExplicitTemp")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0078TemporaryRecordsShouldNotTriggerTableTriggers>();
        fixture.NoDiagnosticAtMarker(code, Rule0078TemporaryRecordsShouldNotTriggerTableTriggers.DiagnosticDescriptors.Rule0078TemporaryRecordsShouldNotTriggerTableTriggers.Id);
    }
}