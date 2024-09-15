#nullable disable // TODO: Enable nullable and review rule
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0008NoFilterOperatorsInSetRange : DiagnosticAnalyzer
    {
        private readonly Lazy<Regex> replacementFieldPatternLazy = new Lazy<Regex>((Func<Regex>)(() => new Regex(@"%\d+", RegexOptions.Compiled)));
        private Regex ReplacementFieldPatternLazy => this.replacementFieldPatternLazy.Value;
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics.DiagnosticDescriptor>(DiagnosticDescriptors.Rule0008NoFilterOperatorsInSetRange);
        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), Microsoft.Dynamics.Nav.CodeAnalysis.OperationKind.InvocationExpression);

        private void AnalyzeInvocation(OperationAnalysisContext context)
        {
            if (context.IsObsoletePendingOrRemoved()) return;
            IInvocationExpression operation = (IInvocationExpression)context.Operation;
            if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "setrange") || operation.TargetMethod == null || operation.Arguments.Count() < 2)
                return;

            CheckParameter(operation.Arguments[1].Value, ref operation, ref context);
            if (operation.Arguments.Count() == 3)
                CheckParameter(operation.Arguments[2].Value, ref operation, ref context);
        }

        private void CheckParameter(IOperation operand, ref IInvocationExpression operation, ref OperationAnalysisContext context)
        {
            if (operand.Type.GetNavTypeKindSafe() != NavTypeKind.String && operand.Type.GetNavTypeKindSafe() != NavTypeKind.Joker)
                return;

            if (operand.Syntax.Kind != SyntaxKind.LiteralExpression)
                return;

            string parameterString = operand.Syntax.ToFullString();

            if ((parameterString.Contains('<') || parameterString.Contains('>') ||
                parameterString.Contains("..") || parameterString.Contains('*') ||
                parameterString.Contains('&') || parameterString.Contains('|')))
            {
                ReportDiagnostic(operation.Syntax.GetLocation(), ref context);
                return;
            }

            Match match = this.ReplacementFieldPatternLazy.Match(parameterString);
            if (match.Success)
                ReportDiagnostic(operation.Syntax.GetLocation(), ref context);
        }

        private void ReportDiagnostic(Microsoft.Dynamics.Nav.CodeAnalysis.Text.Location location, ref OperationAnalysisContext context)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.Rule0008NoFilterOperatorsInSetRange,
                    location));
        }
    }
}
