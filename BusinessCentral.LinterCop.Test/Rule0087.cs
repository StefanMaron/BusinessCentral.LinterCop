namespace BusinessCentral.LinterCop.Test;

public class Rule0087
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0087");
    }

    [Test]
    [TestCase("EmptyStringLiteral")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0087UseIsNullGuid>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0087UseIsNullGuid.Id);
    }

    [Test]
    [TestCase("EmptyStringLiteral")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0087UseIsNullGuid>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0087UseIsNullGuid.Id);
    }
}