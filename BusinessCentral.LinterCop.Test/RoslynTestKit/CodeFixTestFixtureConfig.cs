using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public class CodeFixTestFixtureConfig : BaseTestFixtureConfig
{
    public IReadOnlyCollection<DiagnosticAnalyzer> AdditionalAnalyzers { get; set; } =
        ImmutableArray<DiagnosticAnalyzer>.Empty;
}