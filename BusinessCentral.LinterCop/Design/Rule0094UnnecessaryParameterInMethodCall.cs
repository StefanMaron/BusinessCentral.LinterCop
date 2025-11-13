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

        var semanticModel = context.Compilation.GetSemanticModel(operation.Syntax.SyntaxTree);

        var instance = operation.Instance;
        if (instance?.Type is { NavTypeKind: NavTypeKind.Record })
        {
            CheckMethodCalledFromRecord(context, operation, semanticModel);
            return;
        }

        // method called in current table
        if (instance is null && HelperFunctions.IsOperationInvokedInTable(context, operation))
        {
            CheckMethodCalledInCurrentTable(context, operation, semanticModel);
        }
    }

    private void CheckMethodCalledFromRecord(OperationAnalysisContext context, IInvocationExpression operation, SemanticModel semanticModel)
    {
        var instanceSyntax = operation.Instance?.Syntax;
        if (instanceSyntax == null)
            return;

        var instanceSymbol = semanticModel.GetSymbolInfo(instanceSyntax).Symbol;
        if (instanceSymbol == null)
            return;

        var instanceName = instanceSymbol.Name;
        
        foreach (var argument in operation.Arguments)
        {
            // Quick string check before expensive symbol resolution
            var argText = argument.Syntax.ToString();
            if (!string.Equals(argText, instanceName, StringComparison.OrdinalIgnoreCase))
                continue;

            var argumentSymbol = semanticModel.GetSymbolInfo(argument.Syntax).Symbol;
            if (argumentSymbol != null && instanceSymbol.Equals(argumentSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0094UnnecessaryParameterInMethodCall,
                    argument.Syntax.GetLocation()
                ));
            }
        }
    }

    private void CheckMethodCalledInCurrentTable(OperationAnalysisContext context, IInvocationExpression operation, SemanticModel semanticModel)
    {
        foreach (var arg in operation.Arguments)
        {
            // Quick string check first to avoid expensive symbol resolution
            var argText = arg.Syntax.ToString();
            if (!string.Equals(argText, "Rec", StringComparison.OrdinalIgnoreCase))
                continue;

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