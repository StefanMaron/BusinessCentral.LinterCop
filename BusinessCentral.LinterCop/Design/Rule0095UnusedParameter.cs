using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0095UnusedParameter : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0095UnusedProcedureParameter);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSymbolAction(
            new Action<SymbolAnalysisContext>(this.AnalyzeMethod),
            SymbolKind.Method
        );
    }

    private void AnalyzeMethod(SymbolAnalysisContext context)
    {
        if (context.IsObsoletePendingOrRemoved() || context.Symbol is not IMethodSymbol methodSymbol)
            return;
            
        // If containing object is not an internal object, we do not need to check
        if (methodSymbol.DeclaredAccessibility != Accessibility.Internal)
            return;

        // Skip event publishers and event subscribers
        if (methodSymbol.IsEvent)
            return;

        // Skip if method has no parameters
        if (methodSymbol.Parameters.IsEmpty)
            return;

        // Get method body for analysis
        var syntaxReference = methodSymbol.DeclaringSyntaxReference;
        if (syntaxReference == null)
            return;

        var methodSyntax = syntaxReference.GetSyntax();
        if (methodSyntax == null)
            return;

        foreach (var parameter in methodSymbol.Parameters)
        {
            // Check if parameter is used in method body
            if (!IsParameterUsed(parameter, methodSyntax, context.Compilation))
            {           
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0095UnusedProcedureParameter,
                    parameter.GetLocation(),
                    parameter.Name
                ));
            }
        }
    }

    private static bool IsParameterUsed(IParameterSymbol parameter, SyntaxNode methodSyntax, Compilation compilation)
    {
        var semanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);
        
        // Find all identifier nodes in the method body
        foreach (var node in methodSyntax.DescendantNodes())
        {
            if (!node.IsKind(SyntaxKind.IdentifierName))
                continue;

            // Quick name check first (avoid expensive GetSymbolInfo call)
            var nodeText = node.ToString();
            if (!string.Equals(nodeText, parameter.Name, StringComparison.OrdinalIgnoreCase))
                continue;

            // Skip parameter declarations (parent is Parameter node)
            if (node.Parent?.IsKind(SyntaxKind.Parameter) == true)
                continue;

            // Now check if identifier actually references parameter
            var symbolInfo = semanticModel.GetSymbolInfo(node);
            if (symbolInfo.Symbol is IParameterSymbol paramSymbol && 
                string.Equals(paramSymbol.Name, parameter.Name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}