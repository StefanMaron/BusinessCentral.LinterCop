using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0050SetFilterOperatorCharInFilterExpression : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0050SetFilterOperatorCharInFilterExpression);
        private static readonly List<char> unsupportedOperators = new List<char>
        {
            '*', '?', '@'
        };

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
            foreach (char unsupportedOperator in unsupportedOperators)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                if (parameterString.Contains(unsupportedOperator))
                {
                    ctx.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.Rule0050SetFilterOperatorCharInFilterExpression,
                            operation.Syntax.GetLocation(), new object[] { unsupportedOperator }));
                }
            }
        }
    }
}