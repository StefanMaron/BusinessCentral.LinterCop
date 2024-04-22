using Microsoft.Dynamics.Nav.Analyzers.Common;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Packaging;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    class Rule0033AppManifestRuntimeBehind : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0033AppManifestRuntimeBehind);

        public override void Initialize(AnalysisContext context) => context.RegisterCompilationAction(new Action<CompilationAnalysisContext>(this.CheckAppManifestRuntime));

        private void CheckAppManifestRuntime(CompilationAnalysisContext ctx)
        {
            NavAppManifest manifest = ManifestHelper.GetManifest(ctx.Compilation);

            if (manifest == null) return;
            if (manifest.Runtime == null) return;
            if (manifest.Application == null && manifest.Platform == null) return;

            GetTargetProperty(manifest, out string propertyName, out Version propertyVersion);
            ReleaseVersion.LatestSupportedRuntimeVersions.TryGetValue(propertyVersion, out Version supportedRuntime);
            if (supportedRuntime == null) return;

            if (manifest.Runtime < supportedRuntime)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0033AppManifestRuntimeBehind, manifest.GetDiagnosticLocation("runtime"), new object[] { propertyName, propertyVersion, manifest.Runtime, supportedRuntime }));
        }

        private static void GetTargetProperty(NavAppManifest manifest, out string propertyName, out Version propertyVersion)
        {
            if (manifest.Application >= manifest.Platform)
            {
                propertyName = "application";
                propertyVersion = new Version(manifest.Application.Major, manifest.Application.Minor);
            }
            else
            {
                propertyName = "platform";
                propertyVersion = new Version(manifest.Platform.Major, manifest.Platform.Minor);
            }
        }
    }
}