#if !LessThenFall2024
namespace BusinessCentral.LinterCop.Test;

public class Rule0005Keyword
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0005Keyword");
    }

    [Test]
    [TestCase("Codeunit")]
    [TestCase("Enum")]
    [TestCase("Table")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0005CasingMismatchKeyword>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0005CasingMismatch.Id);
    }

    [Test]
    [TestCase("Codeunit")]
    [TestCase("Enum")]
    [TestCase("Table")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0005CasingMismatchKeyword>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0005CasingMismatch.Id);
    }
}
#endif