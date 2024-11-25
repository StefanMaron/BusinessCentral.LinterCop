using System.Collections.Immutable;
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0026ToolTipPunctuation : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0026ToolTipMustEndWithDot, DiagnosticDescriptors.Rule0036ToolTipShouldStartWithSpecifies, DiagnosticDescriptors.Rule0037ToolTipDoNotUseLineBreaks, DiagnosticDescriptors.Rule0038ToolTipMaximumLength);

        // https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/user-assistance#guidelines-for-tooltip-text
        // Try to not exceed 200 characters including spaces.
        // Including the double quote at the beginning and end of the string, makes this a total of 202
        private const int MaxTooltipLength = 202;

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeToolTipPunctuation), SyntaxKind.PageField, SyntaxKind.PageAction, SyntaxKind.Field);

        private void AnalyzeToolTipPunctuation(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved())
                return;

            var tooltipProperty = ctx.Node.GetPropertyValue(PropertyKind.ToolTip);
            if (tooltipProperty == null)
                return;

            if (tooltipProperty is not LabelPropertyValueSyntax labelPropertyValueSyntax)
                return;

            string tooltipText = labelPropertyValueSyntax.Value.LabelText.Value.ToString();

            CheckEndsWithDot(ctx, tooltipText, tooltipProperty);
            CheckStartsWithSpecifies(ctx, tooltipText, tooltipProperty);
            CheckNoLineBreaks(ctx, tooltipText, tooltipProperty);
            CheckMaximumLength(ctx, tooltipText, tooltipProperty);
        }

        private static void CheckEndsWithDot(SyntaxNodeAnalysisContext ctx, string tooltipText, PropertyValueSyntax tooltipProperty)
        {
            if (!tooltipText.EndsWith(".'", StringComparison.OrdinalIgnoreCase))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0026ToolTipMustEndWithDot, tooltipProperty.GetLocation()));
            }
        }

        private static void CheckStartsWithSpecifies(SyntaxNodeAnalysisContext ctx, string tooltipText, PropertyValueSyntax tooltipProperty)
        {
            if (ctx.ContainingSymbol.Kind == SymbolKind.Control &&
                ((IControlSymbol)ctx.ContainingSymbol).ControlKind == ControlKind.Field &&
                !tooltipText.StartsWith("'Specifies", StringComparison.Ordinal))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0036ToolTipShouldStartWithSpecifies, tooltipProperty.GetLocation()));
            }
        }

        private static void CheckNoLineBreaks(SyntaxNodeAnalysisContext ctx, string tooltipText, PropertyValueSyntax tooltipProperty)
        {
            if (tooltipText.Contains("\\"))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0037ToolTipDoNotUseLineBreaks, tooltipProperty.GetLocation()));
            }
        }
        private static void CheckMaximumLength(SyntaxNodeAnalysisContext ctx, string tooltipText, PropertyValueSyntax tooltipProperty)
        {
            if (tooltipText.Length > MaxTooltipLength)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0038ToolTipMaximumLength, tooltipProperty.GetLocation()));
            }
        }
    }
}