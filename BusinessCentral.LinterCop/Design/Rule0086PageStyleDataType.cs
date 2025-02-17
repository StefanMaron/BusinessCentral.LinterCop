#if !LessThenFall2024
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
        ImmutableArray.Create(DiagnosticDescriptors.Rule0086PageStyleDataType);

    public override VersionCompatibility SupportedVersions => VersionCompatibility.Fall2024OrGreater;

    private static readonly IReadOnlyDictionary<string, string> StyleKindDictionary =
        Enum.GetValues(typeof(StyleKind))
            .Cast<StyleKind>()
            .ToDictionary(
                styleKind => styleKind.ToString(),
                styleKind => styleKind.ToString(),
                StringComparer.Ordinal);

    public override void Initialize(AnalysisContext context) =>
      context.RegisterSyntaxNodeAction(AnalyzeStringLiteralToken, SyntaxKind.StringLiteralValue);

    private void AnalyzeStringLiteralToken(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Node is not StringLiteralValueSyntax stringLiteralNode)
            return;

        if (ctx.ContainingSymbol is IPropertySymbol { PropertyKind: PropertyKind.Caption } ||
            ctx.ContainingSymbol is ISymbol { Kind: SymbolKind.Enum or SymbolKind.EnumValue })
            return;

        var labelSyntax = GetLabelSyntax(stringLiteralNode);
        if (labelSyntax is not null && IsUnlockedLabel(labelSyntax))
            return;

        var stringLiteralValue = stringLiteralNode.Value.Value?.ToString();
        if (string.IsNullOrEmpty(stringLiteralValue))
            return;

        if (StyleKindDictionary.TryGetValue(stringLiteralValue, out string styleKind))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0086PageStyleDataType,
                ctx.Node.GetLocation(),
                stringLiteralValue,
                styleKind));
        }
    }

    private static LabelSyntax? GetLabelSyntax(StringLiteralValueSyntax stringLiteralNode)
    {
        if (stringLiteralNode.GetFirstParent(SyntaxKind.Label) is LabelSyntax parentNode)
            return parentNode;

        return null;
    }

    private static bool IsUnlockedLabel(LabelSyntax labelSyntax)
    {
        // Check if the label has a "Locked" property set to true
        bool isLocked = labelSyntax.Properties?.Values
            .Any(prop => string.Equals(prop.Identifier.ValueText, "Locked", StringComparison.OrdinalIgnoreCase)) ?? false;

        // If it's locked, return false (i.e., not unlocked), otherwise true
        return !isLocked;
    }
}
#endif