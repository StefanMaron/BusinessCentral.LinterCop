using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule9999AssemblyVersionCompatibilityAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptors.Rule9999AssemblyVersionCompatibilityAnalyzer);

    public override void Initialize(AnalysisContext context) =>
            context.RegisterCompilationStartAction(new Action<CompilationStartAnalysisContext>(this.CompareAssemblyVersion));

    private void CompareAssemblyVersion(CompilationStartAnalysisContext startCtx)
    {
        var codeAnalysisAsm = typeof(Compilation).Assembly;
        var codeAnalysisAsmVersion = codeAnalysisAsm.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        var thisAsm = GetThisAnalyzerAssembly<Rule0089CognitiveComplexity>();
        var thisAsmCompatibilityVersion = thisAsm.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        startCtx.RegisterCompilationEndAction(endCtx =>
            ReportDiagnostic(endCtx, codeAnalysisAsmVersion, thisAsmCompatibilityVersion));
    }

    private static void ReportDiagnostic(CompilationAnalysisContext endCtx, string? codeAnalysisVersion, string? thisCompatibilityVersion)
    {
        if (string.IsNullOrEmpty(codeAnalysisVersion) || string.IsNullOrEmpty(thisCompatibilityVersion))
            return;

        if (codeAnalysisVersion != thisCompatibilityVersion)
        {
            endCtx.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.Rule9999AssemblyVersionCompatibilityAnalyzer,
                    Location.None,
                    codeAnalysisVersion,
                    thisCompatibilityVersion)
                    );
        }
    }

    private static Assembly GetThisAnalyzerAssembly<TAnalyzer>()
    {
        var analyzerType = typeof(TAnalyzer);

        // Try directly (should work in normal cases)
        var analyzerAsm = analyzerType.Assembly;
        if (!IsMicrosoftCompilerAssembly(analyzerAsm))
            return analyzerAsm;

        // Fallback
        var resolved = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetType(analyzerType.FullName!, throwOnError: false, ignoreCase: false) is not null);

        return resolved ?? analyzerAsm; //

        static bool IsMicrosoftCompilerAssembly(Assembly a)
        {
            var name = a.GetName().Name ?? string.Empty;
            return name.StartsWith("Microsoft.Dynamics.", StringComparison.OrdinalIgnoreCase)
                && name.EndsWith(".CodeAnalysis", StringComparison.OrdinalIgnoreCase);
        }
    }
}