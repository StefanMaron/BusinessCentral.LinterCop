using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0086PageStyleDataType : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0086PageStyleDataType);

    public override VersionCompatibility SupportedVersions => VersionCompatibility.Fall2024OrGreater;

    private static readonly Dictionary<string, string> styleKindDictionary
        = Enum.GetValues(typeof(StyleKind))
            .Cast<StyleKind>()
            .Select(item => item.ToString())
            .ToDictionary(item => item, item => item, StringComparer.Ordinal);

    public override void Initialize(AnalysisContext context) =>
      context.RegisterSyntaxNodeAction(AnalyzeStringLiteralToken, SyntaxKind.StringLiteralValue);

    private void AnalyzeStringLiteralToken(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Node is not StringLiteralValueSyntax stringLiteralValueSyntax)
            return;

        if (stringLiteralValueSyntax.GetFirstParent(SyntaxKind.Label) is not null)
            return;

        var stringLiteralValue = stringLiteralValueSyntax.Value.Value.ToString();

        // Try to get the value from the dictionary
        if (styleKindDictionary.TryGetValue(stringLiteralValue, out string styleKind))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0086PageStyleDataType,
                ctx.Node.GetLocation(),
                stringLiteralValue,
                styleKind));
        }
    }
}