namespace BusinessCentral.LinterCop.Test;

public class Rule0077
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0077");
    }

    [Test]
    [TestCase("NoBrackets")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0077MissingParenthesis>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0077MissingParenthesis.Id);
    }

    [Test]
    [TestCase("Brackets")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0077MissingParenthesis>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0077MissingParenthesis.Id);
    }
}