namespace BusinessCentral.LinterCop.Test;

public class Rule0030
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0030");
    }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0030AccessInternalForInstallAndUpgradeCodeunits>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0030AccessInternalForInstallAndUpgradeCodeunits.Id);
    }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0030AccessInternalForInstallAndUpgradeCodeunits>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0030AccessInternalForInstallAndUpgradeCodeunits.Id);
    }
}