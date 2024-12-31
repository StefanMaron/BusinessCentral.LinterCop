namespace BusinessCentral.LinterCop.Test;

public class Rule0023
{
    private string _testCaseDir = "";

    [SetUp]
    public void Setup()
    {
        _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
            "TestCases", "Rule0023");
    }

    //TODO: Resolve "There is no issue reported for LC0023 at [96...107]." for these tests.
    // [Test]
    // [TestCase("BrickIsMissing")]
    // [TestCase("DropDownIsMissing")]
    // [TestCase("FieldgroupsIsMissing")]
    // public async Task HasDiagnostic(string testCase)
    // {
    //     var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
    //         .ConfigureAwait(false);

    //     var fixture = RoslynFixtureFactory.Create<BuiltInMethodImplementThroughCodeunit>();
    //     fixture.HasDiagnostic(code, DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups.Id);
    // }

    [Test]
    [TestCase("HasBrickAndDropDown")]
    public async Task NoDiagnostic(string testCase)
    {
        var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
            .ConfigureAwait(false);

        var fixture = RoslynFixtureFactory.Create<BuiltInMethodImplementThroughCodeunit>();
        fixture.NoDiagnosticAtMarker(code, DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups.Id);
    }
}