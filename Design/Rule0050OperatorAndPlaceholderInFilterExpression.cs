using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0050OperatorAndPlaceholderInFilterExpression : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0050OperatorAndPlaceholderInFilterExpression);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), OperationKind.InvocationExpression);

        private void AnalyzeInvocation(OperationAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            if (operation.TargetMethod == null || !SemanticFacts.IsSameName(operation.TargetMethod.Name, "SetFilter") || operation.Arguments.Count() < 2)
                return;

            CheckParameter(operation.Arguments[1].Value, ref operation, ref ctx);
        }

        private void CheckParameter(IOperation operand, ref IInvocationExpression operation, ref OperationAnalysisContext ctx)
        {
            if (operand.Type.GetNavTypeKindSafe() != NavTypeKind.String && operand.Type.GetNavTypeKindSafe() != NavTypeKind.Joker)
                return;

            if (operand.Syntax.Kind != SyntaxKind.LiteralExpression)
                return;

            string parameterString = operand.Syntax.ToFullString();

            string pattern = @"%\d+"; // Only when a placeholders (%1) is used in the filter expression we need to raise the rule that the placeholders won't work as expected
            Regex regex = new Regex(pattern);
            Match match = regex.Match(parameterString);
            if (!match.Success) return;

            int operatorIndex = parameterString.IndexOfAny("*?@".ToCharArray()); // Only the *, ? and @ operator changes the behavior of the placeholder
            if (operatorIndex == -1) return;

            ctx.ReportDiagnostic(
               Diagnostic.Create(
                   DiagnosticDescriptors.Rule0050OperatorAndPlaceholderInFilterExpression,
                   operation.Syntax.GetLocation(), new object[] { parameterString.Substring(operatorIndex, 1), match.Value }));
        }
    }
}