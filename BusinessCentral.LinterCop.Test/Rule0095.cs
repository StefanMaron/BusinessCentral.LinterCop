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
    [TestCase("ProcedureWithUnusedParameters")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0095UnusedParameter>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0095UnusedProcedureParameter.Id);
    }

    [Test]
    [TestCase("InternalProcedureWithUsedParameters")]
    [TestCase("LocalProcedureWithUnusedParameters")]
    [TestCase("GlobalProcedureWithUnusedParameters")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0095UnusedParameter>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0095UnusedProcedureParameter.Id);
    }
}