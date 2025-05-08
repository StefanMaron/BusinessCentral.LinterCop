using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0045ZeroEnumValueReservedForEmpty : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0045ZeroEnumValueReservedForEmpty);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeReservedEnum), SyntaxKind.EnumValue);

    private void AnalyzeReservedEnum(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Node is not EnumValueSyntax enumValue)
            return;

        if (ctx.ContainingSymbol.Kind != SymbolKind.Enum ||
            enumValue.Id.ValueText != "0")
            return;

        if (ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol() is not IEnumTypeSymbol enumTypeSymbol ||
            enumTypeSymbol.ImplementedInterfaces.Any())
            return;

        if (enumValue.GetNameStringValue()?.Trim() != "")
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0045ZeroEnumValueReservedForEmpty,
                enumValue.Name.GetLocation()));

        if (ctx.Node.GetPropertyValue("Caption") is not LabelPropertyValueSyntax captionProperty)
            return;

        if (captionProperty.Value.LabelText.Value.Value.ToString()?.Trim() != "")
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0045ZeroEnumValueReservedForEmpty,
                captionProperty.GetLocation()));
    }
}