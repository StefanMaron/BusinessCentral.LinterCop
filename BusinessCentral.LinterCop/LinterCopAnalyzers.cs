using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop
{
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    internal class LinterCopAnalyzers
    {
        private static ResourceManager? resourceMan;
        private static CultureInfo? resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (LinterCopAnalyzers.resourceMan is null)
                    LinterCopAnalyzers.resourceMan = new ResourceManager("BusinessCentral.LinterCop.LinterCopAnalyzers", typeof(LinterCopAnalyzers).Assembly);
                return LinterCopAnalyzers.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get => LinterCopAnalyzers.resourceCulture ?? CultureInfo.CurrentUICulture;
            set => LinterCopAnalyzers.resourceCulture = value;
        }

        internal static string AnalyzerPrefix => LinterCopAnalyzers.ResourceManager.GetString(nameof(AnalyzerPrefix), LinterCopAnalyzers.resourceCulture);
        internal static string AnalyzerPrefixWithColon => LinterCopAnalyzers.ResourceManager.GetString(nameof(AnalyzerPrefixWithColon), LinterCopAnalyzers.resourceCulture);
        internal static string Fix0001FlowFieldsShouldNotBeEditableCodeAction => $"{AnalyzerPrefixWithColon}{ResourceManager.GetString(nameof(Fix0001FlowFieldsShouldNotBeEditableCodeAction), resourceCulture)}";

        internal static LocalizableString GetLocalizableString(string nameOfLocalizableResource)
        {
            return new LocalizableResourceString(
                nameOfLocalizableResource,
                LinterCopAnalyzers.ResourceManager,
                typeof(LinterCopAnalyzers)
                );
        }
    }
}