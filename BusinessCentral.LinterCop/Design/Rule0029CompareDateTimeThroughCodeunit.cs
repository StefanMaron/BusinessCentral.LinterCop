using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0029CompareDateTimeThroughCodeunit : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0029CompareDateTimeThroughCodeunit);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CompareDateTimeWithTypeHelper), OperationKind.BinaryOperatorExpression);

    private void CompareDateTimeWithTypeHelper(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IBinaryOperatorExpression operation)
            return;

        if (!(operation.LeftOperand.Type.NavTypeKind == NavTypeKind.DateTime &&
            operation.RightOperand.Type.NavTypeKind == NavTypeKind.DateTime))
            return;

        if (operation.LeftOperand.Syntax.IsKind(SyntaxKind.LiteralExpression) &&
            operation.LeftOperand.Syntax.GetIdentifierOrLiteralValue() == "0DT")
            return;

        if (operation.RightOperand.Syntax.IsKind(SyntaxKind.LiteralExpression) &&
            operation.RightOperand.Syntax.GetIdentifierOrLiteralValue() == "0DT")
            return;

        if (operation.Syntax.IsKind(SyntaxKind.EqualsExpression) ||
            operation.Syntax.IsKind(SyntaxKind.NotEqualsExpression) ||
            operation.Syntax.IsKind(SyntaxKind.GreaterThanExpression) ||
            operation.Syntax.IsKind(SyntaxKind.GreaterThanOrEqualExpression) ||
            operation.Syntax.IsKind(SyntaxKind.LessThanExpression) ||
            operation.Syntax.IsKind(SyntaxKind.LessThanOrEqualExpression)
        )
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0029CompareDateTimeThroughCodeunit,
                ctx.Operation.Syntax.GetLocation()));
    }
}