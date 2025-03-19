namespace BusinessCentral.LinterCop.Test;

public class Rule0071
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0071");
    }

    [Test]
    [TestCase("Assignment")]
    [TestCase("Invocation")]
    [TestCase("Handled")]
    [TestCase("PrecedingExitOnAssignment")]
    [TestCase("PrecedingExitOnInvocation")]
    public async Task HasDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0071DoNotSetIsHandledToFalse>();
        fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0071DoNotSetIsHandledToFalse.Id);
    }

    [Test]
    [TestCase("Assignment")]
    [TestCase("Invocation")]
    [TestCase("NoEventSubscriberParameterReference")]
    [TestCase("PrecedingExitOnAssignment")]
    [TestCase("PrecedingExitOnInvocation")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<Rule0071DoNotSetIsHandledToFalse>();
        fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0071DoNotSetIsHandledToFalse.Id);
    }
}