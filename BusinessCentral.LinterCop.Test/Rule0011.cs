namespace BusinessCentral.LinterCop.Test;

public class Rule0011
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0011");
    }

    // FIXME Infos are currently not recognized
    // [Test]
    // [TestCase("1")]
    // public async Task HasDiagnostic(string testCase)
    // {
    //     var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
    //         .ConfigureAwait(false);

    //     var fixture = RoslynFixtureFactory.Create<Rule0011DataPerCompanyShouldAlwaysBeSet>();
    //     fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet.Id);
    // }

    [Test]
    [TestCase("1")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0011DataPerCompanyShouldAlwaysBeSet>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet.Id);
    }
}