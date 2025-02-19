namespace BusinessCentral.LinterCop.Test;

public class Rule0072
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0072");
    }

    [Test]
    [TestCase("Return")]
    [TestCase("Parameter")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0072CheckProcedureDocumentationComment>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0072CheckProcedureDocumentationComment.Id);
    }

    [Test]
    [TestCase("Return")]
    [TestCase("Parameter")]
    [TestCase("NoDocumentationComment")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0072CheckProcedureDocumentationComment>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0072CheckProcedureDocumentationComment.Id);
    }
}