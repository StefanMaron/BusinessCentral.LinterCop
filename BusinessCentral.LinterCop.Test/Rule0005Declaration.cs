namespace BusinessCentral.LinterCop.Test;

public class Rule0005Declaration
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0005Declaration");
    }

    [Test]
#if !LessThenFall2024
    [TestCase("DataType")]
    [TestCase("EnumDataType")]
    [TestCase("FieldGroup")]
    [TestCase("LabelDataType")]
    [TestCase("LabelProperties")]
    [TestCase("LengthDataType")]
    [TestCase("OptionDataType")]
    [TestCase("Property")]
    [TestCase("TextConstDataType")]
    [TestCase("TriggerDeclaration")]
#endif
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0005CasingMismatchDeclaration>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0005CasingMismatch.Id);
    }

    [Test]
#if !LessThenFall2024
    [TestCase("DataType")]
    [TestCase("EnumDataType")]
    [TestCase("FieldGroup")]
    [TestCase("IdentifierNameSyntaxGrouping")]
    [TestCase("LabelDataType")]
    [TestCase("LabelProperties")]
    [TestCase("LengthDataType")]
    [TestCase("OptionDataType")]
    [TestCase("Property")]
    [TestCase("TextConstDataType")]
    [TestCase("TriggerDeclaration")]
#endif
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0005CasingMismatchDeclaration>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0005CasingMismatch.Id);
    }
}