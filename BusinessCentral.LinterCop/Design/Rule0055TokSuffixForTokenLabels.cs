using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0055TokSuffixForTokenLabels : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0055TokSuffixForTokenLabels);

    // https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/analyzers/codecop-aa0074#remarks
    internal static readonly ImmutableHashSet<string> approvedSuffixes = (new string[6]
    {
        "Msg",
        "Tok",
        "Err",
        "Qst",
        "Lbl",
        "Txt"
    }).ToImmutableHashSet<string>();

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeLockedLabel), SymbolKind.GlobalVariable, SymbolKind.LocalVariable);

    private void AnalyzeLockedLabel(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IVariableSymbol variable)
            return;

        if (variable.Type is not ILabelTypeSymbol label)
            return;

        if (label.Locked is not true || label.Name.EndsWith("Tok"))
            return;

        string labelNameNoSuffix = label.Name;

        if (label.Name.Length > 3 && approvedSuffixes.Any(label.Name.EndsWith))
            labelNameNoSuffix = label.Name.Substring(0, label.Name.Length - 3);

        if (labelNameNoSuffix.ToLowerInvariant() == label.Text?.ToLowerInvariant())
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0055TokSuffixForTokenLabels,
                variable.GetLocation()));

            return;
        }

        string labelTextAlphanumeric = string.Join("", (label.Text ?? string.Empty).Where(c => char.IsLetterOrDigit(c)));

        if (labelNameNoSuffix.ToLowerInvariant() == labelTextAlphanumeric.ToLowerInvariant())
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0055TokSuffixForTokenLabels,
                variable.GetLocation()));
    }
}