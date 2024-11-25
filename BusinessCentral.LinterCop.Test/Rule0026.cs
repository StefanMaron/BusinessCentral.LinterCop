namespace BusinessCentral.LinterCop.Test;

public class Rule0026
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0026");
    }

    [Test]
    [TestCase("PageField")]
    [TestCase("PageAction")]
    [TestCase("TableField")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0026ToolTipPunctuation>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0026ToolTipMustEndWithDot.Id);
    }

    [Test]
    [TestCase("PageField")]
    [TestCase("PageAction")]
    [TestCase("TableField")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0026ToolTipPunctuation>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0026ToolTipMustEndWithDot.Id);
    }
}