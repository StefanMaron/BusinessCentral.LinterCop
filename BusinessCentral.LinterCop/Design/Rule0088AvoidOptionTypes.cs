using System;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;


[DiagnosticAnalyzer]
public class Rule0088AvoidOptionTypes : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0088AvoidOptionTypes);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(
            new Action<SyntaxNodeAnalysisContext>(this.Check),
            new SyntaxKind[]{
                    SyntaxKind.OptionDataType
            }
        );
    }

    private void Check(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || (ctx.ContainingSymbol is IMethodSymbol method && method.IsEventSubscriber()))
        {
            return;
        }

        // ignore option types in CDS tables
        if (ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol() is ITableTypeSymbol table)
        {
            if (table.TableType == TableTypeKind.CDS)
            {
                return;
            }
        }

        // TODO: ignore external option types



        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0088AvoidOptionTypes,
            ctx.Node.GetLocation(),
            new object[] { ctx.Node.ToString() }
        ));
    }
}