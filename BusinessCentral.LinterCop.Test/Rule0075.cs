#if !LessThenSpring2024
namespace BusinessCentral.LinterCop.Test;

public class Rule0075
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0075");
    }

    [Test]
    [TestCase("ImplicitConversiontCodeToEnum")]
    [TestCase("ImplicitConversiontEnumToAnotherEnum")]
    [TestCase("RecordGetCodeFieldLengthTooLong")]
    [TestCase("RecordGetGlobalVariable")]
    [TestCase("RecordGetLocalVariable")]
    [TestCase("RecordGetMethod")]
    [TestCase("RecordGetParameter")]
    [TestCase("RecordGetReportDataItem")]
    [TestCase("RecordGetReturnValue")]
    [TestCase("RecordGetSetupTableIncorrectArgumentsProvided")]
    [TestCase("RecordGetSetupTableNoArgumentsProvided")]
    [TestCase("RecordGetXmlPortTableElement")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0075RecordGetProcedureArguments>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0075RecordGetProcedureArguments.Id);
    }

    [Test]
    [TestCase("ImplicitConversiontIntegerToEnum")]
    [TestCase("ImplicitConversiontLabelToCode")]
    [TestCase("RecordGetBuiltInMethodRecordId")]
    [TestCase("RecordGetFieldRecordId")]
    [TestCase("RecordGetGlobalVariable")]
    [TestCase("RecordGetLocalVariable")]
    [TestCase("RecordGetLocalVariableRecordId")]
    [TestCase("RecordGetMethod")]
    [TestCase("RecordGetMethodRecordId")]
    [TestCase("RecordGetParameter")]
    [TestCase("RecordGetParameterRecordId")]
    [TestCase("RecordGetReportDataItem")]
    [TestCase("RecordGetReturnValue")]
    [TestCase("RecordGetReturnValueRecordId")]
    [TestCase("RecordGetSetupTableCorrectArgumentsProvided")]
    [TestCase("RecordGetSetupTableNoArgumentsProvided")]
    [TestCase("RecordGetXmlPortTableElement")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0075RecordGetProcedureArguments>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0075RecordGetProcedureArguments.Id);
    }
}
#endif