using System;
using System.Collections.Immutable;
using System.Linq;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0096UnnecessaryParameterInMethodCall : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [DiagnosticDescriptors.Rule0096UnnecessaryParameterInMethodCall];

    public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(
        new Action<OperationAnalysisContext>(this.AnalyzeInvocation),
        OperationKind.InvocationExpression);

    private void AnalyzeInvocation(OperationAnalysisContext context)
    {
        if (context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending ||
            context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
        if (context.ContainingSymbol.IsObsoletePending || context.ContainingSymbol.IsObsoleteRemoved) return;

        IInvocationExpression operation = (IInvocationExpression)context.Operation;
        // Procedure does not contain arguments -> nothing to check
        if (operation.Arguments.IsEmpty)
            return;

        var instance = operation.Instance;
        if (instance?.Type is { NavTypeKind: NavTypeKind.Record })
        {
            CheckMethodCalledFromRecord(context, operation);
            return;
        }

        // method called in current table
        if (instance is null && HelperFunctions.IsOperationInvokedInTable(context, operation))
        {
            CheckMethodCalledInCurrentTable(context, operation);
        }
    }

    private void CheckMethodCalledFromRecord(OperationAnalysisContext context, IInvocationExpression operation)
    {
        var instanceSyntax = operation.Instance?.Syntax;
        if (instanceSyntax == null)
            return;

        foreach (var arg in operation.Arguments)
        {
            if (arg.Syntax.ToString().Equals(instanceSyntax.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                                            DiagnosticDescriptors.Rule0096UnnecessaryParameterInMethodCall,
                                            arg.Syntax.GetLocation()));
            }
        }
    }

    private void CheckMethodCalledInCurrentTable(OperationAnalysisContext context, IInvocationExpression operation)
    {
        foreach (var arg in operation.Arguments)
        {
            if (arg.Syntax.ToString().ToUpper() == "REC")
            {
                context.ReportDiagnostic(Diagnostic.Create(
                                            DiagnosticDescriptors.Rule0096UnnecessaryParameterInMethodCall,
                                            arg.Syntax.GetLocation()));
            }
        }
    }
}