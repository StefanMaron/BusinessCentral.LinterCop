namespace BusinessCentral.LinterCop.Test;

public class Rule0074
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0074");
    }

    [Test]
    [TestCase("AssignmentStatement")]
    [TestCase("CompoundAssignmentStatement")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0074FlowFilterAssignment>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0074FlowFilterAssignment.Id);
    }

    [Test]
    [TestCase("SetRange")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0074FlowFilterAssignment>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0074FlowFilterAssignment.Id);
    }
}