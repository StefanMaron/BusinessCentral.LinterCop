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

        internal static string AnalyzerPrefix => GetFromResourceManager();
        internal static string Fix0001FlowFieldsShouldNotBeEditableCodeAction => GetFromResourceManager();
        internal static string Fix0077MissingParenthesisCodeAction => GetFromResourceManager();
        internal static string Fix0019DataClassificationFieldEqualsTableCodeAction => GetFromResourceManager();
        internal static string Fix0024SemicolonAfterMethodOrTriggerDeclarationCodeAction => GetFromResourceManager();

        internal static LocalizableString GetLocalizableString(string nameOfLocalizableResource)
        {
            return new LocalizableResourceString(
                nameOfLocalizableResource,
                LinterCopAnalyzers.ResourceManager,
                typeof(LinterCopAnalyzers)
                );
        }

        private static string GetFromResourceManager(
            [CallerMemberName] string? resourceName = null)
        {
            if (resourceName is null)
                throw new ArgumentNullException(nameof(resourceName));

            string? value = LinterCopAnalyzers.ResourceManager
                .GetString(resourceName, LinterCopAnalyzers.resourceCulture);

            if (value is null)
                throw new InvalidOperationException(
                    $"Embedded resource '{resourceName}' not found.");

            return value;
        }
    }
}