using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0026ToolTipPunctuation : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0026ToolTipPunctuation);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeToolTipPunctuation), SyntaxKind.PageField, SyntaxKind.PageAction);

        private void AnalyzeToolTipPunctuation(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;

            PropertyValueSyntax tooltipProperty = ctx.Node.GetPropertyValue(PropertyKind.ToolTip);
            if (tooltipProperty == null)
                return;

            StringLiteralValueSyntax tooltipLabel = ((LabelPropertyValueSyntax)tooltipProperty).Value.LabelText;
            if (!tooltipLabel.Value.ToString().EndsWith(".'"))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0026ToolTipPunctuation, tooltipProperty.GetLocation()));
        }
    }
}