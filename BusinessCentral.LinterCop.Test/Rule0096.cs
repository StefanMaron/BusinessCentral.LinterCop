namespace BusinessCentral.LinterCop.Test;

public class Rule0096
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0096");
    }

    [Test]
    [TestCase("UnneccassaryParameterCalledFromRecord")]
    [TestCase("UnneccassaryParameterInTable")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0096UnnecessaryParameterInMethodCall>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0096UnnecessaryParameterInMethodCall.Id);
    }

    [Test]
    [TestCase("DifferentParameter")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0096UnnecessaryParameterInMethodCall>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0096UnnecessaryParameterInMethodCall.Id);
    }
}