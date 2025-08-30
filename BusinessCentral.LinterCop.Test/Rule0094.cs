using System;
using System.Collections.Generic;
namespace BusinessCentral.LinterCop.Test;

public class Rule0094
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0094");
    }

    [Test]
    [TestCase("GlobalVariableInCodeunit")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0094NoGlobalVariablesInCodeunit>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0094NoGlobalVariablesInCodeunit.Id);
    }

    [Test]
    [TestCase("GlobalLabel")]
    [TestCase("GlobalVaraibleInTable")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0094NoGlobalVariablesInCodeunit>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0094NoGlobalVariablesInCodeunit.Id);
    }
}