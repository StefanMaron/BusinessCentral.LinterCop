using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0045ZeroEnumValueReservedForEmpty : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0045ZeroEnumValueReservedForEmpty);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeReservedEnum), SyntaxKind.EnumValue);

        private void AnalyzeReservedEnum(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;
#if Spring2021
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;

            IEnumTypeSymbol enumTypeSymbol = ctx.ContainingSymbol.GetContainingObjectTypeSymbol() as IEnumTypeSymbol;
            if (enumTypeSymbol != null && enumTypeSymbol.ImplementedInterfaces.Any()) return;
#endif
            LabelPropertyValueSyntax captionProperty = ctx.Node?.GetProperty("Caption")?.Value as LabelPropertyValueSyntax;
            EnumValueSyntax enumValue = ctx.Node as EnumValueSyntax;

            if (enumValue == null) return;

            if (enumValue.Id.ValueText != "0" || ctx.ContainingSymbol.Kind != SymbolKind.Enum) return;

            if (enumValue.GetNameStringValue().Trim() != "")
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0045ZeroEnumValueReservedForEmpty, enumValue.Name.GetLocation()));

            if (captionProperty != null && captionProperty.Value.LabelText.Value.Value.ToString().Trim() != "")
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0045ZeroEnumValueReservedForEmpty, captionProperty.GetLocation()));
        }
    }
}