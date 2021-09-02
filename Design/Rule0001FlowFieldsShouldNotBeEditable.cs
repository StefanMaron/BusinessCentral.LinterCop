using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Readability
{
  [DiagnosticAnalyzer]
  public class Rule0001FlowFieldsShouldNotBeEditable : DiagnosticAnalyzer
  {
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable, DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable);


    public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeFlowFieldEditable), SymbolKind.Field);
        
    private void AnalyzeFlowFieldEditable(SymbolAnalysisContext ctx)
    {
        var isFlowField = "";
        var isEditable = "";
        var LastEditableLocation = ctx.Symbol.GetLocation();
        foreach (IPropertySymbol symbol in ctx.Symbol.Properties)
        {
            if (symbol.PropertyKind == PropertyKind.FieldClass && symbol.ValueText == "FlowField")
                isFlowField = "true";

            if (symbol.PropertyKind == PropertyKind.Editable && symbol.ValueText == "0")
                isEditable = "false";

            if (symbol.PropertyKind == PropertyKind.Editable)
                LastEditableLocation = symbol.GetLocation();
        }
        if (isFlowField == "true" && isEditable != "false")
        {
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable, LastEditableLocation));
        }
    }
  }
}
