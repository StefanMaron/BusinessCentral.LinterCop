using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Text.RegularExpressions;
using System.Threading;


namespace BusinessCentral.LinterCop
{
  internal static class Extensions
  {
    private static readonly Regex CamelCaseRegex = new Regex("^[a-z][a-zA-Z0-9]*$", RegexOptions.Compiled);

    internal static bool IsTestCodeunit(this IApplicationObjectTypeSymbol symbol) => symbol is ICodeunitTypeSymbol codeunitTypeSymbol && codeunitTypeSymbol.Subtype == CodeunitSubtypeKind.Test;

    internal static bool IsUpgradeCodeunit(this IApplicationObjectTypeSymbol symbol) => symbol is ICodeunitTypeSymbol codeunitTypeSymbol && codeunitTypeSymbol.Subtype == CodeunitSubtypeKind.Upgrade;

    internal static bool IsAllowedLowerPermissionObject(this IApplicationObjectTypeSymbol symbol) => symbol.Kind == SymbolKind.Codeunit && (symbol.Id == 132218 && SemanticFacts.IsSameName(symbol.Name, "Permission Test Catalog") || symbol.Id == 132230 && SemanticFacts.IsSameName(symbol.Name, "Library - E2E Role Permissions"));

    internal static int GetTokenLine(this SyntaxToken token) => token.GetLocation().GetMappedLineSpan().StartLinePosition.Line;

    internal static bool IsValidCamelCase(this string str) => !string.IsNullOrEmpty(str) && Extensions.CamelCaseRegex.IsMatch(str);

    internal static IdentifierNameSyntax GetIdentifierNameSyntax(
      this SyntaxNodeAnalysisContext context)
    {
        if (context.Node.IsKind(SyntaxKind.IdentifierName))
        return (IdentifierNameSyntax) context.Node;
        return !context.Node.IsKind(SyntaxKind.IdentifierNameOrEmpty) ? (IdentifierNameSyntax) null : ((IdentifierNameOrEmptySyntax) context.Node).IdentifierName;
    }

    internal static bool TryGetSymbolFromIdentifier(
      SyntaxNodeAnalysisContext syntaxNodeAnalysisContext,
      IdentifierNameSyntax identifierName,
      SymbolKind symbolKind,
      out ISymbol symbol)
    {
        symbol = (ISymbol) null;
        SymbolInfo symbolInfo = syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo((ExpressionSyntax) identifierName, new CancellationToken());
        ISymbol symbol1 = symbolInfo.Symbol;
        if ((symbol1 != null ? (symbol1.Kind != symbolKind ? 1 : 0) : 1) != 0)
        return false;
        symbol = symbolInfo.Symbol;
        return true;
    }
    
    internal static IMethodSymbol? GetFirstMethod(
      this IApplicationObjectTypeSymbol applicationObject,
      string memberName,
      Compilation compilation)
    {
      foreach (ISymbol member in applicationObject.GetMembers(memberName))
      {
        if (member.Kind == SymbolKind.Method)
          return (IMethodSymbol) member;
      }
      foreach (var extensionsAcrossModule in compilation.GetApplicationObjectExtensionTypeSymbolsAcrossModules(applicationObject))
      {
        foreach (var member in extensionsAcrossModule.GetMembers(memberName))
        {
          if (member.Kind == SymbolKind.Method)
          {
            IMethodSymbol firstMethod = (IMethodSymbol) member;
            return firstMethod;
          }
        }
      }
      return null;
    }

  }
}
