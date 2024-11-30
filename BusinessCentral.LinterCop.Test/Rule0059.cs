namespace BusinessCentral.LinterCop.Test;

public class Rule0059
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0059");
    }

    [Test]
    [TestCase("CalcFormulaFieldWhere")]
    [TestCase("CalcFormulaTableWhere")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0059SingleQuoteEscaping>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0059SingleQuoteEscapingIssueDetected.Id);
    }

    [Test]
    [TestCase("CalcFormulaFieldWhere")]
    [TestCase("CalcFormulaTableWhere")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0059SingleQuoteEscaping>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0059SingleQuoteEscapingIssueDetected.Id);
    }
}