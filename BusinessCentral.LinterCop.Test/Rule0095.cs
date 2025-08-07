namespace BusinessCentral.LinterCop.Test;

public class Rule0095
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0095");
    }

    [Test]
    [TestCase("UnneccessaryRecUsageCodeunit")]
    [TestCase("UnneccessaryRecUsageTable")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0095AvoidUnneccessaryRecUsage>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0095AvoidUnneccessaryRecUsage.Id);
    }

    [Test]
    [TestCase("RecUsageInCodeunitWithSourceTable")]
    [TestCase("RecUsageInEventSubscriber")]
    [TestCase("RecUsageInPage")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0095AvoidUnneccessaryRecUsage>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0095AvoidUnneccessaryRecUsage.Id);
    }
}