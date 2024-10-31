#if Spring2024
namespace BusinessCentral.LinterCop.Test;

public class Rule0043
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0043");
    }

    [Test]
    [TestCase("IsolatedStorageGet_1")]
    [TestCase("IsolatedStorageGet_2")]
    [TestCase("IsolatedStorageSet_1")]
    [TestCase("IsolatedStorageSet_2")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0043SecretText>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0043SecretText.Id);
    }

    [Test]
    [TestCase("IsolatedStorageGet_1")]
    [TestCase("IsolatedStorageGet_2")]
    [TestCase("IsolatedStorageSet_1")]
    [TestCase("IsolatedStorageSet_2")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0043SecretText>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0043SecretText.Id);
    }
}
#endif