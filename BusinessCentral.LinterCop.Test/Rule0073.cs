namespace BusinessCentral.LinterCop.Test;

public class Rule0073
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0073");
    }

    [Test]
    [TestCase("BusinessEvent")]
    [TestCase("IntegrationEvent")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0073EventPublisherIsHandledByVar>();
        fixture.HasDiagnostic(code, Rule0073EventPublisherIsHandledByVar.DiagnosticDescriptors.Rule0073EventPublisherIsHandledByVar.Id);
    }

    [Test]
    [TestCase("BusinessEvent")]
    [TestCase("IntegrationEvent")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0073EventPublisherIsHandledByVar>();
        fixture.NoDiagnosticAtMarker(code, Rule0073EventPublisherIsHandledByVar.DiagnosticDescriptors.Rule0073EventPublisherIsHandledByVar.Id);
    }
}