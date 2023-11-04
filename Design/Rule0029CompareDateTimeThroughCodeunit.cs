using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0029CompareDateTimeThroughCodeunit : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0029CompareDateTimeThroughCodeunit);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CompareDateTimeWithTypeHelper), OperationKind.BinaryOperatorExpression);

        private void CompareDateTimeWithTypeHelper(OperationAnalysisContext context)
        {
            if (context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (context.ContainingSymbol.IsObsoletePending || context.ContainingSymbol.IsObsoleteRemoved) return;

            IBinaryOperatorExpression operation = (IBinaryOperatorExpression)context.Operation;

            if (operation.LeftOperand.Type.NavTypeKind == NavTypeKind.DateTime && operation.RightOperand.Type.NavTypeKind == NavTypeKind.DateTime &&
                (operation.Syntax.IsKind(SyntaxKind.EqualsExpression) ||
                 operation.Syntax.IsKind(SyntaxKind.NotEqualsExpression) ||
                 operation.Syntax.IsKind(SyntaxKind.GreaterThanExpression) ||
                 operation.Syntax.IsKind(SyntaxKind.GreaterThanOrEqualExpression) ||
                 operation.Syntax.IsKind(SyntaxKind.LessThanExpression) ||
                 operation.Syntax.IsKind(SyntaxKind.LessThanOrEqualExpression)))
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0029CompareDateTimeThroughCodeunit, context.Operation.Syntax.GetLocation()));
        }
    }
}