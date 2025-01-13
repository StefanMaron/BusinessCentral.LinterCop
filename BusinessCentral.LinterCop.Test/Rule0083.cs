namespace BusinessCentral.LinterCop.Test;

#if !LessThenFall2024
public class Rule0083
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0083");
    }

    [Test]
    [TestCase("Date2DMY")]
    [TestCase("Date2DWY")]
    [TestCase("DT2Date")]
    [TestCase("DT2Time")]
    [TestCase("FormatHour")]
    [TestCase("FormatMillisecond")]
    [TestCase("FormatMinute")]
    [TestCase("FormatSecond")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0083BuiltInDateTimeMethod>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod.Id);
    }

    // [Test]
    // public async Task NoDiagnostic(string testCase)
    // {
    //     var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
    //         .ConfigureAwait(false);

    //     var fixture = RoslynFixtureFactory.Create<Rule0083BuiltInDateTimeMethod>();
    //     fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod.Id);
    // }
}
#endif