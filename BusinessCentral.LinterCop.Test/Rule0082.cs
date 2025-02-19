namespace BusinessCentral.LinterCop.Test;

public class Rule0082
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0082");
    }

    [Test]
    [TestCase("RecordCountEqualsOne")]
    [TestCase("RecordCountGreaterThanOne")]
    [TestCase("RecordCountGreaterThanOrEqualOne")]
    [TestCase("RecordCountLessThanOrEqualZero")]
    [TestCase("RecordCountLessThanTwo")]
    [TestCase("RecordCountNotEqualsOne")]
    [TestCase("TwoGreaterThanRecordCount")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0081AnalyzeCountMethod>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0082UseFindWithNext.Id);
    }

    [Test]
    [TestCase("RecordCountEqualsTwo")]
    [TestCase("RecordTemporaryCountEqualsOne")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0081AnalyzeCountMethod>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0082UseFindWithNext.Id);
    }
}