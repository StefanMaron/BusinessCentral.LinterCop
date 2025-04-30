using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Packaging;

#if !LessThenSpring2024
using Microsoft.Dynamics.Nav.Analyzers.Common;

#else
using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;

#endif

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
class Rule0033AppManifestRuntimeBehind : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0033AppManifestRuntimeBehind);

    private static readonly SortedList<Version, Version> SupportedRuntimeVersionList = GetSupportedRuntimeVersions();

    public override void Initialize(AnalysisContext context) =>
        context.RegisterCompilationAction(new Action<CompilationAnalysisContext>(this.CheckAppManifestRuntime));

    private void CheckAppManifestRuntime(CompilationAnalysisContext ctx)
    {
#if !LessThenSpring2024
        NavAppManifest? manifest = ManifestHelper.GetManifest(ctx.Compilation);
#else
        NavAppManifest? manifest = AppSourceCopConfigurationProvider.GetManifest(ctx.Compilation);
#endif

        // In the case the runtime version isn't specified in the app.json it returns the RuntimeVersion.CurrentRelease
        if (manifest is null || manifest.Runtime is null || manifest.Runtime == RuntimeVersion.CurrentRelease)
            return;

        if (manifest.Application is null && manifest.Platform is null)
            return;

        GetTargetProperty(manifest, out string propertyName, out Version propertyVersion);
        Version supportedRuntime = FindValueOfFirstValueLessThan(SupportedRuntimeVersionList, propertyVersion);
        if (supportedRuntime is null)
            return;

        if (manifest.Runtime < supportedRuntime)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0033AppManifestRuntimeBehind,
                manifest.GetDiagnosticLocation("runtime"),
                propertyName,
                propertyVersion,
                manifest.Runtime,
                supportedRuntime));
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

    private static SortedList<Version, Version> GetSupportedRuntimeVersions()
    {
        // Populate a SortedList with platform version and runtime version combined
        SortedList<Version, Version> AvailableRuntimeVersion = new SortedList<Version, Version>();

        // https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/devenv-choosing-runtime#currently-available-runtime-versions  
        // When in the future the offset between the platform and runtime versions isn't exactly is eleven, we're in trouble
        // By populating a list here, instead of just adding up eleven somewhere else, we probably can resolve this here
        int offset = 11;

        foreach (var v in RuntimeVersion.SupportedVersions)
        {
            AvailableRuntimeVersion.Add(new Version(v.Major + offset, v.Minor), new Version(v.Major, v.Minor));
        }
        return AvailableRuntimeVersion;
    }

    private static Version FindValueOfFirstValueLessThan(SortedList<Version, Version> sortedList, Version version)
    {
        int index = FindIndexOfFirstValueLessThan(sortedList.Keys.ToList(), version);
        return sortedList.ElementAtOrDefault(index).Value;
    }

    private static int FindIndexOfFirstValueLessThan<T>(List<T> sortedList, T value, IComparer<T>? comparer = null)
    {
        var index = sortedList.BinarySearch(value, comparer);

        // The value was found in the list. Just return its index.
        if (index >= 0)
            return index;

        // The value was not found and "~index" is the index of the next value greater than the search value.
        index = ~index;

        // There are values in the list less than the search value. Return the index of the closest one.
        if (index > 0)
            return index - 1;

        // All values in the list are greater than the search value.
        return -1;
    }
}