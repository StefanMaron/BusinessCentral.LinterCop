using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Semantics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0085LFSeparator : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0085LFSeparator);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeAssignmentStatement), OperationKind.AssignmentStatement);

    private void AnalyzeAssignmentStatement(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IAssignmentStatement operation)
            return;

        if (IsLFSeparatorAssignment(operation))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                 DiagnosticDescriptors.Rule0085LFSeparator,
                 ctx.Operation.Syntax.GetLocation()));
        }
    }

    private static bool IsLFSeparatorAssignment(IAssignmentStatement operation)
    {
        // Ensure the right side of the assignment is the literal value "10"
        if (!IsValidLFSeparatorValue(operation.Value))
            return false;

        // Validate the left-hand side for specific cases
        return IsValidLFSeparatorTarget(operation.Target);
    }

    private static bool IsValidLFSeparatorValue(IOperation valueOperation)
    {
        return valueOperation.Syntax is LiteralExpressionSyntax sourceLiteral &&
               sourceLiteral.Literal is Int32SignedLiteralValueSyntax sourceInt &&
               sourceInt.GetIdentifierOrLiteralValue() == "10";
    }

    private static bool IsValidLFSeparatorTarget(IOperation targetOperation)
    {
        return targetOperation.Kind switch
        {
            // Case: Code[1], Code[2], Text[1], Text[2]
            OperationKind.FieldAccess => IsValidTextOrCodeArrayAccess(targetOperation),

            // Case: Char variable
            OperationKind.LocalReferenceExpression or OperationKind.GlobalReferenceExpression =>
                IsValidCharVariable(targetOperation),

            _ => false
        };
    }

    private static bool IsValidTextOrCodeArrayAccess(IOperation targetOperation)
    {
        if (targetOperation.Syntax is not ElementAccessExpressionSyntax elementAccess ||
            elementAccess.ArgumentList.Arguments.Count != 1 ||
            elementAccess.ArgumentList.Arguments[0] is not LiteralExpressionSyntax indexLiteral ||
            indexLiteral.Literal is not Int32SignedLiteralValueSyntax indexInt)
        {
            return false;
        }

        var indexValue = indexInt.GetIdentifierOrLiteralValue();
        return indexValue == "1" || indexValue == "2";
    }

    private static bool IsValidCharVariable(IOperation targetOperation)
    {
        return targetOperation.Syntax.Kind == SyntaxKind.IdentifierName &&
               targetOperation.GetSymbol() is IVariableSymbol variableSymbol &&
               variableSymbol.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.Char;
    }
}