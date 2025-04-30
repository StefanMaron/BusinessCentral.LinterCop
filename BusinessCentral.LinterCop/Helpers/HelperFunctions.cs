using Microsoft.Dynamics.Nav.CodeAnalysis;

namespace BusinessCentral.LinterCop.Helpers;

public class HelperFunctions
{
    public static bool MethodImplementsInterfaceMethod(IMethodSymbol methodSymbol)
    {
        return MethodImplementsInterfaceMethod(methodSymbol.GetContainingApplicationObjectTypeSymbol(), methodSymbol);
    }

    public static bool MethodImplementsInterfaceMethod(IApplicationObjectTypeSymbol? objectSymbol, IMethodSymbol methodSymbol)
    {
        if (objectSymbol is not ICodeunitTypeSymbol codeunitSymbol)
        {
            return false;
        }

        foreach (var implementedInterface in codeunitSymbol.ImplementedInterfaces)
        {
            if (implementedInterface.GetMembers().OfType<IMethodSymbol>().Any(interfaceMethodSymbol => MethodImplementsInterfaceMethod(methodSymbol, interfaceMethodSymbol)))
            {
                return true;
            }
        }

        return false;
    }

    public static bool MethodImplementsInterfaceMethod(IMethodSymbol methodSymbol, IMethodSymbol interfaceMethodSymbol)
    {
        if (methodSymbol.Name != interfaceMethodSymbol.Name)
        {
            return false;
        }
        if (methodSymbol.Parameters.Length != interfaceMethodSymbol.Parameters.Length)
        {
            return false;
        }
        var methodReturnValType = methodSymbol.ReturnValueSymbol?.ReturnType.NavTypeKind ?? NavTypeKind.None;
        var interfaceMethodReturnValType = interfaceMethodSymbol.ReturnValueSymbol?.ReturnType.NavTypeKind ?? NavTypeKind.None;
        if (methodReturnValType != interfaceMethodReturnValType)
        {
            return false;
        }
        for (int i = 0; i < methodSymbol.Parameters.Length; i++)
        {
            var methodParameter = methodSymbol.Parameters[i];
            var interfaceMethodParameter = interfaceMethodSymbol.Parameters[i];

            if (methodParameter.IsVar != interfaceMethodParameter.IsVar)
            {
                return false;
            }
            if (!methodParameter.ParameterType.Equals(interfaceMethodParameter.ParameterType))
            {
                return false;
            }
        }
        return true;
    }
}
