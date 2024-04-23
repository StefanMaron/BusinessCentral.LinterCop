using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.AnalysisContextExtension
{
    public static class AnalysisContextExtensions
    {
        public static bool IsObsoletePendingOrRemoved(this SymbolAnalysisContext context)
        {
            if (context.Symbol.IsObsoletePendingOrRemoved())
            {
                return true;
            }
            if (context.Symbol.GetContainingObjectTypeSymbol().IsObsoletePendingOrRemoved())
            {
                return true;
            }
            return false;
        }

        public static bool IsObsoletePendingOrRemoved(this OperationAnalysisContext context)
        {
            if (context.ContainingSymbol.IsObsoletePendingOrRemoved())
            {
                return true;
            }
            if (context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePendingOrRemoved())
            {
                return true;
            }
            return false;
        }

        public static bool IsObsoletePendingOrRemoved(this SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol.IsObsoletePendingOrRemoved())
            {
                return true;
            }
            if (context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePendingOrRemoved())
            {
                return true;
            }
            return false;
        }

        public static bool IsObsoletePendingOrRemoved(this CodeBlockAnalysisContext context)
        {
            if (context.OwningSymbol.IsObsoletePendingOrRemoved())
            {
                return true;
            }
            if (context.OwningSymbol.GetContainingObjectTypeSymbol().IsObsoletePendingOrRemoved())
            {
                return true;
            }
            return false;
        }

        public static bool IsObsoletePendingOrRemoved(this ISymbol symbol)
        {
            return symbol.IsObsoletePending || symbol.IsObsoleteRemoved;
        }
    }
}