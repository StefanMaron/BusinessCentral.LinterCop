using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0070ListObjectsAreOneBased : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0070ListObjectsAreOneBased);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.Analyze), OperationKind.InvocationExpression);

    private void Analyze(OperationAnalysisContext ctx)
    {
        if (ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.TargetMethod.ContainingType?.GetNavTypeKindSafe() != NavTypeKind.List)
            return;

        switch (operation.TargetMethod.Name)
        {
            case "Get":
                AnalyzeGetOperator(operation, ctx);
                break;

            case "Count":
                AnalyzeCountOperator(operation, ctx);
                break;
        }
    }

    private static void AnalyzeGetOperator(IInvocationExpression operation, OperationAnalysisContext ctx)
    {
        if (operation.Arguments.Length < 1)
            return;

        switch (operation.Arguments[0].Syntax)
        {
            case LiteralExpressionSyntax literalExpressionSyntax:
                if (literalExpressionSyntax.Literal.GetLiteralValue().ToString() == "0")
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.Rule0070ListObjectsAreOneBased,
                        operation.Syntax.GetLocation()));
                }
                break;
        }
    }

    private static void AnalyzeCountOperator(IInvocationExpression operation, OperationAnalysisContext ctx)
    {
        if (operation.Syntax.Parent is not ForStatementSyntax statementSyntax)
            return;

        if (statementSyntax.InitialValue is not LiteralExpressionSyntax expressionSyntax)
            return;

        if (expressionSyntax.Literal is not Int32SignedLiteralValueSyntax valueSyntax)
            return;

        if (valueSyntax.Number.ValueText == "0")
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0070ListObjectsAreOneBased,
                valueSyntax.GetLocation()));
        }
    }
}