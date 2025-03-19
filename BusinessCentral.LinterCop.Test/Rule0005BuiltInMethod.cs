namespace BusinessCentral.LinterCop.Test;

public class Rule0005BuiltInMethod
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0005BuiltInMethod");
    }

    [Test]
    [TestCase("FieldAccess")]
    [TestCase("GlobalReferenceExpression")]
    [TestCase("InvocationExpression")]
    [TestCase("LocalReferenceExpression")]
    [TestCase("ParameterReferenceExpression")]
    [TestCase("ReturnValueReferenceExpression")]
    [TestCase("XmlPortDataItemAccess")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0005CasingMismatchBuiltInMethod>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0005CasingMismatch.Id);
    }

    [Test]
    [TestCase("FieldAccess")]
    [TestCase("GlobalReferenceExpression")]
    [TestCase("InvocationExpression")]
    [TestCase("LocalReferenceExpression")]
    [TestCase("ParameterReferenceExpression")]
    [TestCase("ReturnValueReferenceExpression")]
    [TestCase("XmlPortDataItemAccess")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0005CasingMismatchBuiltInMethod>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0005CasingMismatch.Id);
    }
}