namespace BusinessCentral.LinterCop.Test;

public class Rule0079
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0079");
    }

    [Test]
    [TestCase("PublicEvent")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0079NonPublicEventPublisher>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0079NonPublicEventPublisher.Id);
    }

    [Test]
    [TestCase("LocalEvent")]
    [TestCase("InternalEvent")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0079NonPublicEventPublisher>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0079NonPublicEventPublisher.Id);
    }
}