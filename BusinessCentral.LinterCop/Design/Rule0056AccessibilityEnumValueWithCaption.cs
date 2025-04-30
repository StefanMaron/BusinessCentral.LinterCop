using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0056AccessibilityEnumValueWithCaption : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0056EmptyEnumValueWithCaption, DiagnosticDescriptors.Rule0057EnumValueWithEmptyCaption);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeEnumWithCaption), SyntaxKind.EnumValue);

    private void AnalyzeEnumWithCaption(SyntaxNodeAnalysisContext ctx)
    {
        // Prevent possible duplicate diagnostic
        if (ctx.ContainingSymbol.ContainingType is null)
            return;

        if (ctx.IsObsoletePendingOrRemoved() || ctx.Node is not EnumValueSyntax enumValue)
            return;

        if (ctx.Node?.GetProperty("Caption")?.Value is not LabelPropertyValueSyntax captionProperty)
            return;

        if (enumValue.GetNameStringValue() == "" && captionProperty.Value.LabelText.Value.Value.ToString() != "")
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0056EmptyEnumValueWithCaption,
                ctx.Node.GetLocation()));

        if (captionProperty.Value.Properties?.Values.Where(prop => prop.Identifier.Text.Equals("Locked", StringComparison.OrdinalIgnoreCase)).FirstOrDefault() is not null)
            return;

        if (enumValue.GetNameStringValue() != "" && captionProperty.Value.LabelText.Value.Value.ToString() == "")
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0057EnumValueWithEmptyCaption,
                ctx.Node.GetLocation()));
    }
}