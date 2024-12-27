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
        // Right side needs to be := 10;
        if (operation.Value.Syntax is not LiteralExpressionSyntax sourceLiteral)
            return false;

        if (sourceLiteral.Literal is not Int32SignedLiteralValueSyntax sourceInt)
            return false;

        if (sourceInt.GetIdentifierOrLiteralValue() != "10")
            return false;

        // Left side needs to be Code[1], Text[1] or a Char
        switch (operation.Target.Kind)
        {
            case OperationKind.FieldAccess:
                if (operation.Target.Syntax is not ElementAccessExpressionSyntax elementAccess ||
                    elementAccess.ArgumentList.Arguments.Count != 1 ||
                    elementAccess.ArgumentList.Arguments[0] is not LiteralExpressionSyntax targetLiteral)
                    return false;

                if (targetLiteral.Literal is not Int32SignedLiteralValueSyntax targetInt)
                    return false;

                return targetInt.GetIdentifierOrLiteralValue() == "1";

            case OperationKind.LocalReferenceExpression:
            case OperationKind.GlobalReferenceExpression:
                if (operation.Target.Syntax.Kind != SyntaxKind.IdentifierName ||
                    operation.Target.GetSymbol() is not IVariableSymbol identifierNameSymbol)
                    return false;

                return identifierNameSymbol.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.Char;
        }

        return false;
    }
}