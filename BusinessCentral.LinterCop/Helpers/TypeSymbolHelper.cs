using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

namespace BusinessCentral.LinterCop.Helpers;

public static class TypeSymbolExtensions
{
    internal static IMethodSymbol? GetFirstMethod(this IApplicationObjectTypeSymbol applicationObject, string memberName, Compilation compilation)
    {
        foreach (ISymbol member in applicationObject.GetMembers(memberName))
        {
            if (member.Kind == SymbolKind.Method)
                return (IMethodSymbol)member;
        }
        foreach (var extensionsAcrossModule in compilation.GetApplicationObjectExtensionTypeSymbolsAcrossModules(applicationObject))
        {
            foreach (var member in extensionsAcrossModule.GetMembers(memberName))
            {
                if (member.Kind == SymbolKind.Method)
                {
                    IMethodSymbol firstMethod = (IMethodSymbol)member;
                    return firstMethod;
                }
            }
        }
        return null;
    }

    internal static int GetTypeLength(this ITypeSymbol type, ref bool isError)
    {
        if (!type.IsTextType())
        {
            isError = true;
            return 0;
        }
        if (type.HasLength)
            return type.Length;
        return type.NavTypeKind == NavTypeKind.Label ? GetLabelTypeLength(type) : int.MaxValue;
    }

    private static int GetLabelTypeLength(ITypeSymbol type)
    {
        ILabelTypeSymbol labelType = (ILabelTypeSymbol)type;

        if (labelType.Locked)
            return labelType.GetLabelText().Length;

        return labelType.MaxLength;
    }
}