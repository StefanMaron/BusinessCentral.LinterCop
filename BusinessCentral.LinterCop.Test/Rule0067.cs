namespace BusinessCentral.LinterCop.Test;

public class Rule0067
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0067");
    }

    [Test]
    [TestCase("1")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0067CheckObjectPermission>();
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 8);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 9);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 11);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 12);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 14);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 15);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 16);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 17);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 18);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 20);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 21);
        fixture.HasDiagnosticAtLine(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id, 22);
    }

    [Test]
    [TestCase("1")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0067CheckObjectPermission>();
        fixture.NoDiagnosticAtMarker(code, Rule0067CheckObjectPermission.DiagnosticDescriptors.Rule0067CheckObjectPermission.Id);
    }
}
