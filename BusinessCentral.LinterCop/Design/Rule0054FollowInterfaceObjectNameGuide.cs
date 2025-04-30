using System.Collections.Immutable;
using System.Text;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0054FollowInterfaceObjectNameGuide : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0054FollowInterfaceObjectNameGuide);

    private static IEnumerable<string>? Affixes = null;
    private static readonly char CharOfCapitalI = 'I';

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(new Action<CompilationStartAnalysisContext>(this.PopulateListOfAffixes));
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeObjectName), SymbolKind.Interface);
    }

    private void AnalyzeObjectName(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IInterfaceTypeSymbol interfaceTypeSymbol)
            return;

        // The interface object should start with a capital 'I' and should not have a space after it
        if (interfaceTypeSymbol.Name.StartsWith(CharOfCapitalI) && !char.IsWhiteSpace(interfaceTypeSymbol.Name[1]))
            return;

        int? indexAfterAffix = GetIndexAfterAffix(interfaceTypeSymbol.Name);
        if (indexAfterAffix is null)
        {
            ReportDiagnostic(ctx, interfaceTypeSymbol);
            return;
        }

        string objectNameWithoutPrefix = interfaceTypeSymbol.Name.Remove(0, indexAfterAffix.GetValueOrDefault());

        // The first character after the prefix should be a capital 'I'
        if (RemoveSpecialCharacters(objectNameWithoutPrefix)[0] != CharOfCapitalI)
        {
            ReportDiagnostic(ctx, interfaceTypeSymbol);
            return;
        }

        // The character after the capital 'I' should not be a whitespace
        int index = objectNameWithoutPrefix.IndexOf(CharOfCapitalI);
        if (index != -1 && index < objectNameWithoutPrefix.Length - 1)
        {
            if (char.IsWhiteSpace(objectNameWithoutPrefix[index + 1]))
            {
                ReportDiagnostic(ctx, interfaceTypeSymbol);
                return;
            }
        }
    }

    private void PopulateListOfAffixes(CompilationStartAnalysisContext context)
    {
        Affixes = GetAffixes(context.Compilation);
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
        foreach (var affix in Affixes ?? Enumerable.Empty<string>())
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

    private static List<string>? GetAffixes(Compilation compilation)
    {
        AppSourceCopConfiguration? copConfiguration = AppSourceCopConfigurationProvider.GetAppSourceCopConfiguration(compilation);
        if (copConfiguration is null)
            return null;

        List<string> affixes = new List<string>();
        if (!string.IsNullOrEmpty(copConfiguration.MandatoryPrefix) && !affixes.Contains(copConfiguration.MandatoryPrefix, StringComparer.OrdinalIgnoreCase))
            affixes.Add(copConfiguration.MandatoryPrefix);

        if (copConfiguration.MandatoryAffixes is not null)
        {
            foreach (string mandatoryAffix in copConfiguration.MandatoryAffixes)
            {
                if (!string.IsNullOrEmpty(mandatoryAffix) && !affixes.Contains(mandatoryAffix, StringComparer.OrdinalIgnoreCase))
                    affixes.Add(mandatoryAffix);
            }
        }
        return affixes;
    }

    private void ReportDiagnostic(SymbolAnalysisContext context, IInterfaceTypeSymbol interfaceTypeSymbol)
    {
        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0054FollowInterfaceObjectNameGuide, interfaceTypeSymbol.GetLocation()));
    }
}