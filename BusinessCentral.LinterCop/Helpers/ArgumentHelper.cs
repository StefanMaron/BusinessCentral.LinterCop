using Microsoft.Dynamics.Nav.CodeAnalysis;

namespace BusinessCentral.LinterCop.Helpers;

public static class ArgumentExtensions
{
    public static ITypeSymbol? GetTypeSymbol(this IArgument argument)
    {
        switch (argument.Value.Kind)
        {
            case OperationKind.ConversionExpression:
                return ((IConversionExpression)argument.Value).Operand.Type;
            case OperationKind.InvocationExpression:
                return ((IInvocationExpression)argument.Value).TargetMethod.ReturnValueSymbol.ReturnType;
        }
        return null;
    }
}