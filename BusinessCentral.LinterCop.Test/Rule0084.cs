namespace BusinessCentral.LinterCop.Test;

public class Rule0084
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0084");
    }

    [Test]
    [TestCase("GetMethod")]
    [TestCase("GetBySystemIdMethod")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0084UseReturnValueForErrorHandling>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0084UseReturnValueForErrorHandling.Id);
    }

    [Test]
    [TestCase("GetMethod")]
    [TestCase("GetBySystemIdMethod")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0084UseReturnValueForErrorHandling>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0084UseReturnValueForErrorHandling.Id);
    }
}