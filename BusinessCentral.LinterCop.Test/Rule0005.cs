namespace BusinessCentral.LinterCop.Test;

public class Rule0005
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0005");
    }

    [Test]
    [TestCase("1")]
    [TestCase("2")]
    [TestCase("LabelPropertyComment")]
    [TestCase("LabelPropertyLocked")]
    [TestCase("LabelPropertyMaxLength")]
    [TestCase("MemberAccessExpressionCodeunit")]
    [TestCase("MemberAccessExpressionDatabase")]
    [TestCase("MemberAccessExpressionEnum")]
    [TestCase("MemberAccessExpressionPage")]
    [TestCase("MemberAccessExpressionQuery")]
    [TestCase("MemberAccessExpressionReport")]
    [TestCase("MemberAccessExpressionXmlport")]
    [TestCase("OptionAccessExpressionCodeunit")]
    [TestCase("OptionAccessExpressionDatabase")]
    [TestCase("OptionAccessExpressionPage")]
    [TestCase("OptionAccessExpressionQuery")]
    [TestCase("OptionAccessExpressionReport")]
    [TestCase("OptionAccessExpressionXmlport")]
    [TestCase("PropertyName")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0005VariableCasingShouldNotDifferFromDeclaration>();
        fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration.Id);
    }

    [Test]
    [TestCase("1")]
    [TestCase("LabelProperty")]
    [TestCase("MemberAccessExpression")]
    [TestCase("OptionAccessExpression")]
    [TestCase("PropertyName")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0005VariableCasingShouldNotDifferFromDeclaration>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0005VariableCasingShouldNotDifferFromDeclaration.Id);
    }
}