using System;
using System.Collections.Immutable;
using System.Linq;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0094UnnecessaryParameterInMethodCall : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0094UnnecessaryParameterInMethodCall);

    public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(
        new Action<OperationAnalysisContext>(this.AnalyzeInvocation),
        OperationKind.InvocationExpression);

    private void AnalyzeInvocation(OperationAnalysisContext context)
    {
        if (context.IsObsoletePendingOrRemoved() || context.Operation is not IInvocationExpression operation)
            return;

        // Procedure does not contain arguments -> nothing to check
        if (operation.Arguments.IsEmpty)
            return;

        // ignore Event publisher
        if (operation.TargetMethod is IMethodSymbol methodSymbol && methodSymbol.IsEvent)
            return;

        // ignore methods called in another module
        var currentModule = context.Compilation.ModuleName;
        var targetModule = operation.TargetMethod?.ContainingModule?.Name;
        if (currentModule == null || !string.Equals(currentModule, targetModule))
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

        var semanticModel = context.Compilation.GetSemanticModel(instanceSyntax.SyntaxTree);
        var instanceSymbol = semanticModel.GetSymbolInfo(instanceSyntax).Symbol;

        if (instanceSymbol == null)
            return;

        foreach (var argument in operation.Arguments)
        {
            var argumentSymbol = semanticModel.GetSymbolInfo(argument.Syntax).Symbol;

            if (argumentSymbol != null &&
                instanceSymbol.Equals(argumentSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0094UnnecessaryParameterInMethodCall,
                    argument.Syntax.GetLocation()
                ));
            }
        }
    }

    private void CheckMethodCalledInCurrentTable(OperationAnalysisContext context, IInvocationExpression operation)
    {
        foreach (var arg in operation.Arguments)
        {
            var semanticModel = context.Compilation.GetSemanticModel(arg.Syntax.SyntaxTree);
            var symbolInfo = semanticModel.GetSymbolInfo(arg.Syntax).Symbol;

            if (symbolInfo != null && string.Equals(symbolInfo.Name, "Rec", StringComparison.OrdinalIgnoreCase))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0094UnnecessaryParameterInMethodCall,
                    arg.Syntax.GetLocation()
                ));
            }
        }
    }
}