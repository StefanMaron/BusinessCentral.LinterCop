using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Packaging;
using System.Collections.Immutable;
#if ManifestHelper
using Microsoft.Dynamics.Nav.Analyzers.Common;
#else
using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;
#endif


namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    class Rule0033AppManifestRuntimeBehind : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0033AppManifestRuntimeBehind);

        public override void Initialize(AnalysisContext context) => context.RegisterCompilationAction(new Action<CompilationAnalysisContext>(this.CheckAppManifestRuntime));

        private void CheckAppManifestRuntime(CompilationAnalysisContext ctx)
        {
#if ManifestHelper
            NavAppManifest manifest = ManifestHelper.GetManifest(ctx.Compilation);
#else
            NavAppManifest manifest = AppSourceCopConfigurationProvider.GetManifest(ctx.Compilation);
#endif

            if (manifest == null) return;
            if (manifest.Runtime == null || manifest.Runtime == RuntimeVersion.CurrentRelease) return; // In the case the runtime version isn't specified in the app.json it returns the RuntimeVersion.CurrentRelease
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