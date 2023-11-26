using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0008NoFilterOperatorsInSetRange : DiagnosticAnalyzer
    {
        public override ImmutableArray<Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics.DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics.DiagnosticDescriptor>(DiagnosticDescriptors.Rule0008NoFilterOperatorsInSetRange);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), Microsoft.Dynamics.Nav.CodeAnalysis.OperationKind.InvocationExpression);

        private void AnalyzeInvocation(OperationAnalysisContext context)
        {
            if (context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (context.ContainingSymbol.IsObsoletePending || context.ContainingSymbol.IsObsoleteRemoved) return;
            IInvocationExpression operation = (IInvocationExpression)context.Operation;
            if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "setrange") || operation.TargetMethod == null || operation.Arguments.Count() < 2)
                return;

            CheckParameter(operation.Arguments[1].Value, ref operation, ref context);
            if (operation.Arguments.Count() == 3)
                CheckParameter(operation.Arguments[2].Value, ref operation, ref context);
        }

        private void CheckParameter(Microsoft.Dynamics.Nav.CodeAnalysis.IOperation operand, ref IInvocationExpression operation, ref OperationAnalysisContext context)
        {
            if (operand.Type.GetNavTypeKindSafe() != NavTypeKind.String && operand.Type.GetNavTypeKindSafe() != NavTypeKind.Joker)
                return;

            if (operand.Syntax.Kind != SyntaxKind.LiteralExpression)
                return;

            string parameterString = operand.Syntax.ToFullString();
            if (!(parameterString.Contains('<') || parameterString.Contains('>') ||
                parameterString.Contains("..") || parameterString.Contains('*') ||
                parameterString.Contains('&') || parameterString.Contains('|')))
            {
                return;
            }

            context.ReportDiagnostic(
                Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics.Diagnostic.Create(
                    DiagnosticDescriptors.Rule0008NoFilterOperatorsInSetRange,
                    operation.Syntax.GetLocation()));
        }
    }

}
