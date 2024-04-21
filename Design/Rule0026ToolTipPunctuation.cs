﻿using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0026ToolTipPunctuation : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0026ToolTipMustEndWithDot, DiagnosticDescriptors.Rule0036ToolTipShouldStartWithSpecifies, DiagnosticDescriptors.Rule0037ToolTipDoNotUseLineBreaks, DiagnosticDescriptors.Rule0038ToolTipMaximumLength);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeToolTipPunctuation), SyntaxKind.PageField, SyntaxKind.PageAction);

        private void AnalyzeToolTipPunctuation(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;
#if Spring2021
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
#endif
            PropertyValueSyntax tooltipProperty = ctx.Node.GetPropertyValue(PropertyKind.ToolTip);
            if (tooltipProperty == null) return;

            StringLiteralValueSyntax tooltipLabel = ((LabelPropertyValueSyntax)tooltipProperty).Value.LabelText;
            if (!tooltipLabel.Value.ToString().EndsWith(".'"))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0026ToolTipMustEndWithDot, tooltipProperty.GetLocation()));

            if (ctx.ContainingSymbol.Kind == SymbolKind.Control && ((IControlSymbol)ctx.ContainingSymbol).ControlKind == ControlKind.Field)
                if (!tooltipLabel.Value.ToString().StartsWith("'Specifies"))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0036ToolTipShouldStartWithSpecifies, tooltipProperty.GetLocation()));

            if (tooltipLabel.Value.ToString().Contains("\\"))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0037ToolTipDoNotUseLineBreaks, tooltipProperty.GetLocation()));

            if (tooltipLabel.Value.ToString().Length > 202)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0038ToolTipMaximumLength, tooltipProperty.GetLocation()));
        }
    }
}