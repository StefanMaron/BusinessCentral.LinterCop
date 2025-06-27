using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Semantics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Helpers;

public static class OperationHelper
{
    public static ITypeSymbol? TryGetTargetTypeSymbol(IOperation operation, Compilation compilation)
    {
        return operation switch
        {
            IAssignmentStatement { Target.Type: ITypeSymbol targetType } => targetType,
            IExitStatement => TryGetReturnTypeFromContainingMethod(operation, compilation),
            IInvocationExpression invocation => TryGetTypeFromInvocation(invocation),
            _ => null
        };
    }

    private static ITypeSymbol? TryGetTypeFromInvocation(IInvocationExpression invocation)
    {
        if (invocation.Arguments.Length < 1)
            return null;

        var arg = invocation.Arguments[0].Value;

        if (arg is IConversionExpression conversion)
            arg = conversion.Operand;

        return arg.Type;
    }

    public static ITypeSymbol? TryGetReturnTypeFromContainingMethod(IOperation operation, Compilation compilation)
    {
        if (operation.Syntax.GetFirstParent(SyntaxKind.MethodDeclaration) is not MethodDeclarationSyntax methodDeclaration)
            return null;

        var semanticModel = compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
        var returnValueSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration.ReturnValue);

        return returnValueSymbol?.GetTypeSymbol();
    }
}