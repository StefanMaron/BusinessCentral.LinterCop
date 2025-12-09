using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Text.RegularExpressions;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

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
            if (MethodImplementsInterfaceMethod(implementedInterface, methodSymbol))
            {
                return true;
            }
        }

        return false;
    }

    public static bool MethodImplementsInterfaceMethod(IInterfaceTypeSymbol interfaceTypeSymbol, IMethodSymbol methodSymbol)
    {
        if (interfaceTypeSymbol.GetMembers().OfType<IMethodSymbol>().Any(interfaceMethodSymbol => MethodImplementsInterfaceMethod(methodSymbol, interfaceMethodSymbol)))
        {
            return true;
        }
#if !LessThenFall2024
        foreach (var extendedInterface in interfaceTypeSymbol.ExtendedInterfaces)
        {
            if (MethodImplementsInterfaceMethod(extendedInterface, methodSymbol))
            {
                return true;
            }
        }
#endif

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

    public static bool IsOperationInvokedInTable(OperationAnalysisContext context, IOperation operation)
    {
        var containing = operation?.Syntax?.GetContainingObjectSyntax();
        return containing is TableSyntax;
    }
    
    public static void CheckMatchesPattern(SymbolAnalysisContext ctx, Location location, Regex pattern, string patternSource, string name, string indentifierKind)
    {
        CheckPattern(ctx, location, pattern, patternSource, true, name, indentifierKind);
    }

    public static void CheckDoesNotMatchPattern(SymbolAnalysisContext ctx, Location location, Regex pattern, string patternSource, string name, string indentifierKind)
    {
        CheckPattern(ctx, location, pattern, patternSource, false, name, indentifierKind);
    }

    private static void CheckPattern(SymbolAnalysisContext ctx, Location location, Regex pattern, string patternSource, bool isMatch, string name, string indentifierKind)
    {
        bool matches;
        try
        {
            matches = pattern.IsMatch(name);
        }
        catch (RegexMatchTimeoutException)
        {
            return;
        }
        catch (Exception)
        {
            return;
        }

        if (matches != isMatch)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0092NamesPattern,
                location,
                [
                    indentifierKind,
                    name,
                    isMatch ? "must" : "must not",
                    patternSource,
                    pattern.ToString(),
                ]
            ));
        }
    }
}
