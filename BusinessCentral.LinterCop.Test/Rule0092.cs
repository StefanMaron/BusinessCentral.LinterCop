using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessCentral.LinterCop.Helpers;

namespace BusinessCentral.LinterCop.Test;

[TestFixture]
[NonParallelizable] // LinterSettings is static/global -> — don't run in parallel.
public class Rule0092
{
    private string _testCaseDir = "";
    private string _settingsDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0092");

        _settingsDir = Path.Combine(Path.GetTempPath(), "LinterCopTests", Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(_settingsDir);

        File.WriteAllText(Path.Combine(_settingsDir, "LinterCop.json"),
            """
                {
                    "procedure.name" : {
                        "allow.pattern": "^[A-Z][A-Za-z0-9_]*$", // No special characters allowed (exept "_") & must start with upper case letter
                        "disallow.pattern": "^A42.*", // prodeure names must not start with A42
                        "global.procedure.disallow.pattern": "^Global.*", // global procedure names must not start with Global 
                        "local.procedure.allow.pattern": "^.*_loc$" // local procedure names have to end with _loc
                    },
                    "variable.name" : {
                        "allow.pattern": "^[A-Z][A-Za-z0-9]*$", // No special characters allowed & must start with upper case letter
                        "disallow.pattern": "^A42.*", // variable and parameter names must not start with A42
                    }
                }
            """);

        LinterSettings.Create(_settingsDir);
    }


    [TearDown]
    public void Cleanup()
    {
        try { if (Directory.Exists(_settingsDir)) Directory.Delete(_settingsDir, true); } catch { }
    }

    [Test]
    [TestCase("GlobalProcedureWithDisallowedChars")]
    [TestCase("LocalProcedureWithoutAllowPattern")]
    [TestCase("LowerCaseStart")]
    [TestCase("SpecialCharacters")]
    [TestCase("StartWithDisallowedChars")]
    [TestCase("ParameterWithSpecialCharacters")]
    [TestCase("ReturnParameterWithSpecialCharacter")]
    [TestCase("VariableWithSpecialCharacter")]
    [TestCase("VariableWithDisallowPattern")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0092NamePattern>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0092NamesPattern.Id);
    }

    [Test]
    [TestCase("LocalProcedureWithGlobalDisallowPattern")]
    [TestCase("GlobalProcedureWithoutLocalAllowPattern")]
    [TestCase("ObsoleteLowerCaseStart")]
    [TestCase("VariableNameWithoutSpecialCharacters")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0092NamePattern>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0092NamesPattern.Id);
    }
}
