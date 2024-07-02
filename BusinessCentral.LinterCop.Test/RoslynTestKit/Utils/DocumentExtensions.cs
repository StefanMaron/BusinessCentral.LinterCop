using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit.Utils;

internal static class DocumentExtensions
{
    public static IReadOnlyList<Diagnostic> GetErrors(this Document document)
    {
        var diagnostics = document.GetSemanticModelAsync().GetAwaiter().GetResult().GetDiagnostics();
        return diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
    }
}