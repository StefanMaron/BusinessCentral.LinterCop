using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0075PageStyleDataType : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0075PageStyleDataType);

    private static readonly Dictionary<string, string> _styleKindDictionary
        = Enum.GetValues(typeof(StyleKind))
            .Cast<StyleKind>()
            .Select(item => item.ToString())
            .ToDictionary(item => item, item => item, StringComparer.Ordinal);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeStringLiteralToken, SyntaxKind.StringLiteralValue);
    }

    private void AnalyzeStringLiteralToken(SyntaxNodeAnalysisContext ctx)
    {
        if (!VersionChecker.IsSupported(ctx.ContainingSymbol, VersionCompatibility.Fall2024OrGreater))
            return;

        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Node is not StringLiteralValueSyntax stringLiteralValueSyntax)
            return;

        if (!_styleKindDictionary.TryGetValue(stringLiteralValueSyntax.Value.Value.ToString(), out string styleKind))
            return;

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0075PageStyleDataType,
            ctx.Node.GetLocation(), new object[] { stringLiteralValueSyntax.Value.Value.ToString(), styleKind }));
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0075PageStyleDataType = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0075",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0075PageStyleDataTypeTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0075PageStyleDataTypeFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0075PageStyleDataTypeDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0075");
    }
}