using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0087UseIsNullGuid : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0087UseIsNullGuid);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeEqualsStatement), OperationKind.BinaryOperatorExpression);

    private void AnalyzeEqualsStatement(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IBinaryOperatorExpression operation)
            return;

        // Validate the left operand is a Guid type
        if (!IsNavTypeKindGuid(operation.LeftOperand))
            return;

        // Validate the right operand is an empty string literal
        if (IsEmptyStringLiteral(operation.RightOperand))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0087UseIsNullGuid,
                operation.RightOperand.Syntax.GetLocation(),
                GetVariableName(operation.LeftOperand),
                "''"));
        }
    }

    private static bool IsNavTypeKindGuid(IOperation operand)
    {
        // Check for a conversion expression with a valid Guid type
        if (operand.Kind == OperationKind.ConversionExpression &&
            operand is IConversionExpression conversion &&
            conversion.Operand is IOperation innerOperation)
        {
            return innerOperation.Type.GetNavTypeKindSafe() == NavTypeKind.Guid;
        }

        return false;
    }

    private static bool IsEmptyStringLiteral(IOperation operand)
    {
        // Ensure the operand is a literal expression of a string type
        if (operand.Kind == OperationKind.LiteralExpression && operand.Type.IsTextType())
        {
            var constantValue = operand.ConstantValue.Value?.ToString();
            return string.IsNullOrEmpty(constantValue);
        }

        return false;
    }

    private static string GetVariableName(IOperation operand)
    {
        // Extract the name of the Guid variable, or return an empty string if unavailable
        if (operand.Kind == OperationKind.ConversionExpression &&
            operand is IConversionExpression conversion &&
            conversion.Operand is IOperation innerOperation)
        {
            return innerOperation.GetSymbol()?.Name.QuoteIdentifierIfNeeded() ?? string.Empty;
        }

        return string.Empty;
    }
}