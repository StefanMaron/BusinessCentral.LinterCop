using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0055TokSuffixForTokenLabels : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0055TokSuffixForTokenLabels);

        // https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/analyzers/codecop-aa0074#remarks
        internal static readonly ImmutableHashSet<string> approvedSuffixes = ((IEnumerable<string>)new string[6]
        {
            "Msg",
            "Tok",
            "Err",
            "Qst",
            "Lbl",
            "Txt"
        }).ToImmutableHashSet<string>();

        public override void Initialize(AnalysisContext context) => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeLockedLabel), SymbolKind.GlobalVariable, SymbolKind.LocalVariable);

        private void AnalyzeLockedLabel(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IVariableSymbol variable = (IVariableSymbol)ctx.Symbol;
            if (variable.Type == null || variable.Type.GetNavTypeKindSafe() != NavTypeKind.Label)
                return;

            ILabelTypeSymbol label = variable.Type as ILabelTypeSymbol;
            if (!label.Locked || label.Name.EndsWith("Tok")) return;

            string labelNameNoSuffix = label.Name;
            if (label.Name.Length > 3 && approvedSuffixes.Any(label.Name.EndsWith))
                labelNameNoSuffix = label.Name.Substring(0, label.Name.Length - 3);

            if (labelNameNoSuffix.ToLowerInvariant() == label.GetLabelText().ToLowerInvariant())
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0055TokSuffixForTokenLabels, variable.GetLocation()));
                return;
            }

            string LabelTextAlphanumeric = String.Join("", label.GetLabelText().Where(c => Char.IsLetterOrDigit(c)));
            if (labelNameNoSuffix.ToLowerInvariant() == LabelTextAlphanumeric.ToLowerInvariant())
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0055TokSuffixForTokenLabels, variable.GetLocation()));
        }
    }
}