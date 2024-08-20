namespace BusinessCentral.LinterCop.Test;

public class Rule0068
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0068");
    }

    [Test]
    [TestCase("ProcedureCalls")]
    [TestCase("XmlPorts")]
    [TestCase("Queries")]
    [TestCase("Reports")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0068CheckObjectPermission>();
        fixture.HasDiagnostic(code, Rule0068CheckObjectPermission.DiagnosticDescriptors.Rule0068CheckObjectPermission.Id);
    }

    [Test]
    [TestCase("ProcedureCallsPermissionsProperty")]
    [TestCase("XmlPortPermissionsProperty")]
    [TestCase("QueryPermissionsProperty")]
    [TestCase("XmlPortInherentPermissions")]
    [TestCase("QueryInherentPermissions")]
    [TestCase("ReportPermissionsProperty")]
    [TestCase("ReportInherentPermissions")]
    [TestCase("ProcedureCallsInherentPermissionsProperty")]
    [TestCase("ProcedureCallsInherentPermissionsAttribute")]
    [TestCase("PageSourceTable")]
    [TestCase("ProcedureCallsPermissionsPropertyFullyQualified")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0068CheckObjectPermission>();
        fixture.NoDiagnosticAtMarker(code, Rule0068CheckObjectPermission.DiagnosticDescriptors.Rule0068CheckObjectPermission.Id);
    }
}
