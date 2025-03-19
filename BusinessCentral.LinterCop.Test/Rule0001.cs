namespace BusinessCentral.LinterCop.Test;

public class Rule0001
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0001");
    }

    [Test]
    [TestCase("1")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0001FlowFieldsShouldNotBeEditable>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable.Id);
    }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    [TestCase("3")]
    [TestCase("4")]
    [TestCase("5")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0001FlowFieldsShouldNotBeEditable>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable.Id);
    }
}