using System;
using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0094NoGlobalVariablesInCodeunit : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0094NoGlobalVariablesInCodeunit);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSymbolAction(
            new Action<SymbolAnalysisContext>(this.Check),
            new SymbolKind[]{
                    SymbolKind.GlobalVariable,
            }
        );
    }

    private void Check(SymbolAnalysisContext ctx)
    {
        var containingObject = ctx.Symbol.GetContainingObjectTypeSymbol();
        if (containingObject.Kind != SymbolKind.Codeunit)
        {
            return;
        }

        if (ctx.Symbol is IVariableSymbol variableDeclaration)
        {
            if (variableDeclaration.Type.NavTypeKind == NavTypeKind.Label)
            {
                // Global labels are fine.
                return;
            }

            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0094NoGlobalVariablesInCodeunit,
                ctx.Symbol.GetLocation(),
                [ctx.Symbol.Name]
            ));
        }
    }
}