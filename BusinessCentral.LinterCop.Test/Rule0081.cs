namespace BusinessCentral.LinterCop.Test;

public class Rule0081
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0081");
    }

    [Test]
    [TestCase("OneGreaterThanRecordCount")]
    [TestCase("RecordCountEqualsZero")]
    [TestCase("RecordCountGreaterThanOrEqualZero")]
    [TestCase("RecordCountGreaterThanZero")]
    [TestCase("RecordCountLessThanOne")]
    [TestCase("RecordCountLessThanOrEqualZero")]
    [TestCase("RecordCountLessThanZero")]
    [TestCase("RecordCountNotEqualsZero")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0081AnalyzeCountMethod>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0081UseIsEmptyMethod.Id);
    }

    [Test]
    [TestCase("RecordCountEqualsOne")]
    [TestCase("RecordTemporaryCountEqualsZero")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0081AnalyzeCountMethod>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0081UseIsEmptyMethod.Id);
    }
}