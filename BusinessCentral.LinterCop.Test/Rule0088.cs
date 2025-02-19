using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.LinterCop.Test
{
    internal class Rule0088
    {
        private string _testCaseDir = "";

        [SetUp]
        public void Setup()
        {
            _testCaseDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName,
                "TestCases", "Rule0088");
        }

        [Test]
        [TestCase("OptionField")]
        [TestCase("OptionParameterGlobalVar")]
        [TestCase("OptionParameterLocalVar")]
        [TestCase("OptionReturnValue")]
        [TestCase("OptionVariable")]
        public async Task HasDiagnostic(string testCase)
        {
            var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "HasDiagnostic", $"{testCase}.al"))
                .ConfigureAwait(false);

            var fixture = RoslynFixtureFactory.Create<Rule0088AvoidOptionTypes>();
            fixture.HasDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0088AvoidOptionTypes.Id);
        }

        [Test]
        [TestCase("EventSubscriberOption")]
        [TestCase("ObsoleteFieldOption")]
        [TestCase("CDSDocument")]
        // [TestCase("OptionParameter")] //TODO: See remarks in the test file
        public async Task NoDiagnostic(string testCase)
        {
            var code = await File.ReadAllTextAsync(Path.Combine(_testCaseDir, "NoDiagnostic", $"{testCase}.al"))
                .ConfigureAwait(false);

            var fixture = RoslynFixtureFactory.Create<Rule0088AvoidOptionTypes>();
            fixture.NoDiagnosticAtAllMarkers(code, DiagnosticDescriptors.Rule0088AvoidOptionTypes.Id);
        }
    }
}
