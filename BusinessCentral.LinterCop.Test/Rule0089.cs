namespace BusinessCentral.LinterCop.Test;

public class Rule0089
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0089");
    }

    [Test]
#if !LessThenFall2024
    [TestCase("ConditionalExpressionNested")]
#endif
    [TestCase("IfStatement")]
    [TestCase("IfStatementNested")]
    [TestCase("RecursionDirect")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0089CognitiveComplexity>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0090CognitiveComplexity.Id);
    }

    [Test]
    [TestCase("CurrReportGuardClause")]
    [TestCase("CurrXMLportGuardClause")]
    [TestCase("IfStatement")]
    [TestCase("DiscountConsecutiveAndOperator")]
    [TestCase("IfStatementElseIf")]
    [TestCase("IfStatementGuardClause")]
#if !LessThenFall2025 // TODO: Change to LessThenSpring2025 when AL version 15.0 is no longer in Pre-Release
    [TestCase("IfStatementGuardClauseContinue")]
#endif
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0089CognitiveComplexity>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0090CognitiveComplexity.Id);
    }
}