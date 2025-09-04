using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class AssemblyVersionCompatibilityAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptors.AssemblyVersionCompatibilityMismatch);

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
                    DiagnosticDescriptors.AssemblyVersionCompatibilityMismatch,
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

        return resolved ?? analyzerAsm;

        static bool IsMicrosoftCompilerAssembly(Assembly a)
        {
            var name = a.GetName().Name ?? string.Empty;
            return name.StartsWith("Microsoft.Dynamics.", StringComparison.OrdinalIgnoreCase)
                && name.EndsWith(".CodeAnalysis", StringComparison.OrdinalIgnoreCase);
        }
    }

    private static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor AssemblyVersionCompatibilityMismatch = new(
            id: "LinterCop",
            title: LinterCopAnalyzers.GetLocalizableString("AssemblyVersionCompatibilityMismatchTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("AssemblyVersionCompatibilityMismatchFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Error,
#if DEBUG // The AssemblyFileVersion property is only set in Release builds though the pipeline, so we disable this check in Debug builds
            isEnabledByDefault: false,
#else
            isEnabledByDefault: true,
#endif
            description: LinterCopAnalyzers.GetLocalizableString("AssemblyVersionCompatibilityMismatchDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop");
    }
}