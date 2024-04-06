using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0056AccessibilityEnumValueWithCaption : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0056EmptyEnumValueWithCaption, DiagnosticDescriptors.Rule0057EnumValueWithEmptyCaption);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeEnumWithCaption), SyntaxKind.EnumValue);

        private void AnalyzeEnumWithCaption(SyntaxNodeAnalysisContext ctx)
        {
            // Prevent possible duplicate diagnostic
            if (ctx.ContainingSymbol.ContainingType is null) return;

            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            EnumValueSyntax enumValue = ctx.Node as EnumValueSyntax;
            if (enumValue == null) return;
            LabelPropertyValueSyntax captionProperty = ctx.Node?.GetProperty("Caption")?.Value as LabelPropertyValueSyntax;
            if (captionProperty == null) return;

            if (enumValue.GetNameStringValue() == "" && captionProperty.Value.LabelText.Value.Value.ToString() != "")
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0056EmptyEnumValueWithCaption, ctx.Node.GetLocation()));

            if (captionProperty.Value.Properties?.Values.Where(prop => prop.Identifier.Text.ToLowerInvariant() == "locked").FirstOrDefault() != null) return;

            if (enumValue.GetNameStringValue() != "" && captionProperty.Value.LabelText.Value.Value.ToString() == "")
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0057EnumValueWithEmptyCaption, ctx.Node.GetLocation()));
        }
    }
}