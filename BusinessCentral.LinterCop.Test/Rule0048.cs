namespace BusinessCentral.LinterCop.Test;

public class Rule0048
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0048");
    }

#if !LessThenSpring2024
    [Test]
    [TestCase("ErrorWithLiteralExpression")]
    [TestCase("ErrorWithStrSubstNo")]
    [TestCase("ErrorWithTextVariable")]
#endif
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0048ErrorWithTextConstant>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0048ErrorWithTextConstant.Id);
    }

    [Test]
    [TestCase("ErrorWithErrorInfo")]
    [TestCase("ErrorWithLabel")]
    [TestCase("ErrorWiththisLabel")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0048ErrorWithTextConstant>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0048ErrorWithTextConstant.Id);
    }
}