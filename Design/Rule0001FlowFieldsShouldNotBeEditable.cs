using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0001FlowFieldsShouldNotBeEditable : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable);

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeFlowFieldEditable), SymbolKind.Field);

        private void AnalyzeFlowFieldEditable(SymbolAnalysisContext ctx)
        {
            if (ctx.Symbol.IsObsoletePending || ctx.Symbol.IsObsoleteRemoved) return;
            if (ctx.Symbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.Symbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;

            IFieldSymbol field = (IFieldSymbol)ctx.Symbol;
            if (field.FieldClass == FieldClassKind.FlowField && field.GetBooleanPropertyValue(PropertyKind.Editable).Value)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0001FlowFieldsShouldNotBeEditable, field.Location));
        }
    }
}
