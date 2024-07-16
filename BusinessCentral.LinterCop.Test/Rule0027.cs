namespace BusinessCentral.LinterCop.Test;

public class Rule0027
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0027");
    }

    // [Test]
    // [TestCase("1")]
    // public async Task HasDiagnostic(string testCase)
    // {
    //     var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
    //         .ConfigureAwait(false);
    //
    //     var fixture = RoslynFixtureFactory.Create<Rule0027RunPageImplementPageManagement>();
    //     fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0027RunPageImplementPageManagement.Id);
    // }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0027RunPageImplementPageManagement>();
        fixture.NoDiagnosticAtMarker(code, Rule0027RunPageImplementPageManagement.DiagnosticDescriptors.Rule0027RunPageImplementPageManagement.Id);
    }
}