using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Text;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0054FollowInterfaceObjectNameGuide : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0054FollowInterfaceObjectNameGuide);

        private static IEnumerable<string> _affixes;
        private static readonly char _charCapitalI = 'I';

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(new Action<CompilationStartAnalysisContext>(this.PopulateListOfAffixes));
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeObjectName), SymbolKind.Interface);
        }

        private void AnalyzeObjectName(SymbolAnalysisContext context)
        {
            if (context.IsObsoletePendingOrRemoved()) return;

            if (context.Symbol is not IInterfaceTypeSymbol interfaceTypeSymbol)
                return;

            // The interface object should start with a capital 'I' and should not have a space after it
            if (_affixes is null && interfaceTypeSymbol.Name.StartsWith(_charCapitalI) && !char.IsWhiteSpace(interfaceTypeSymbol.Name[1]))
                return;

            int? indexAfterAffix = GetIndexAfterAffix(interfaceTypeSymbol.Name);
            if (indexAfterAffix is null)
            {
                ReportDiagnostic(context, interfaceTypeSymbol);
                return;
            }

            string objectNameWithoutPrefix = interfaceTypeSymbol.Name.Remove(0, indexAfterAffix.GetValueOrDefault());

            // The first character after the prefix should be a capital 'I'
            if (RemoveSpecialCharacters(objectNameWithoutPrefix)[0] != _charCapitalI)
            {
                ReportDiagnostic(context, interfaceTypeSymbol);
                return;
            }

            // The character after the capital 'I' should not be a whitespace
            int index = objectNameWithoutPrefix.IndexOf(_charCapitalI);
            if (index != -1 && index < objectNameWithoutPrefix.Length - 1)
            {
                if (char.IsWhiteSpace(objectNameWithoutPrefix[index + 1]))
                {
                    ReportDiagnostic(context, interfaceTypeSymbol);
                    return;
                }
            }
        }

        private void PopulateListOfAffixes(CompilationStartAnalysisContext context)
        {
            _affixes = GetAffixes(context.Compilation);
        }

        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static int? GetIndexAfterAffix(string typeSymbolName)
        {
            foreach (var affix in _affixes ?? Enumerable.Empty<string>())
            {
                if (typeSymbolName.StartsWith(affix, StringComparison.OrdinalIgnoreCase))
                {
                    int affixLength = affix.Length;
                    if (typeSymbolName.Length > affixLength)
                    {
                        return affixLength;
                    }
                }
            }

            // Return null if no affix is found or no character is present after the affix
            return null;
        }

        private static List<string> GetAffixes(Compilation compilation)
        {
            AppSourceCopConfiguration copConfiguration = AppSourceCopConfigurationProvider.GetAppSourceCopConfiguration(compilation);
            if (copConfiguration is null)
                return null;

            List<string> affixes = new List<string>();
            if (!string.IsNullOrEmpty(copConfiguration.MandatoryPrefix) && !affixes.Contains(copConfiguration.MandatoryPrefix, StringComparer.OrdinalIgnoreCase))
                affixes.Add(copConfiguration.MandatoryPrefix);

            foreach (string mandatoryAffix in copConfiguration.MandatoryAffixes)
            {
                if (!string.IsNullOrEmpty(mandatoryAffix) && !affixes.Contains(mandatoryAffix, StringComparer.OrdinalIgnoreCase))
                    affixes.Add(mandatoryAffix);
            }
            return affixes;
        }

        private void ReportDiagnostic(SymbolAnalysisContext context, IInterfaceTypeSymbol interfaceTypeSymbol)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0054FollowInterfaceObjectNameGuide, interfaceTypeSymbol.GetLocation()));
        }
    }
}