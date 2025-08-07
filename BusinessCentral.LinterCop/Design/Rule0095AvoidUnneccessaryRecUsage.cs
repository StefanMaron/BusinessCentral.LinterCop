using System;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0095AvoidUnneccessaryRecUsage : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0095AvoidUnneccessaryRecUsage);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterOperationAction(
            new Action<OperationAnalysisContext>(CheckRecNotUsed),
            new OperationKind[] {
                    OperationKind.InvocationExpression,
                    OperationKind.FieldAccess
            });
    }

    private void CheckRecNotUsed(OperationAnalysisContext context)
    {
        if (context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending ||
                context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
        if (context.ContainingSymbol.IsObsoletePending || context.ContainingSymbol.IsObsoleteRemoved) return;

        // In event subscribers "Rec." usage has to be allowed, because variable names have to be taken
        // over from respective publisher.
        if (context.ContainingSymbol is IMethodSymbol method && method.IsEventSubscriber())
        {
            return;
        }

        if (context.Operation.Kind == OperationKind.InvocationExpression)
        {
            IInvocationExpression operation = (IInvocationExpression)context.Operation;
            if (operation.Instance == null || !HelperFunctions.IsRecord(operation.Instance.Type))
                // procedure not called from record -> nothing to check
                return;

            // In pages everything is allowed -> nothing to check 
            // in codeunit with source table everything is allowed -> nothing to check
            if (IsPageOrCodeunitWithSourceTable(context, operation))
                return;

            var operationInstanceSyntax = operation.Instance.Syntax;
            if (operationInstanceSyntax.ToString().ToUpper() != "REC")
                return;

            context.ReportDiagnostic(Diagnostic.Create(
                                        DiagnosticDescriptors.Rule0095AvoidUnneccessaryRecUsage,
                                        operationInstanceSyntax.GetLocation()));
        }

        if (context.Operation is IFieldAccess fieldAccess)
        {
            if (fieldAccess.Instance == null || !HelperFunctions.IsRecord(fieldAccess.Instance.Type))
                // not called from record -> nothing to check
                return;

            // In pages everything is allowed -> nothing to check 
            // in codeunit with source table everything is allowed -> nothing to check
            if (IsPageOrCodeunitWithSourceTable(context, fieldAccess))
                return;

            // field access not from record -> nothing to check
            if (fieldAccess.Instance.Syntax.ToString().ToUpper() != "REC")
                return;

            context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0095AvoidUnneccessaryRecUsage,
                    fieldAccess.Instance.Syntax.GetLocation()));
        }
    }

    private bool IsPageOrCodeunitWithSourceTable(OperationAnalysisContext context, IOperation operation)
    {
        if (operation.Syntax.GetContainingObjectSyntax().GetType().ToString().Contains("Page"))
            return true;

        if (operation.Syntax.GetContainingObjectSyntax().GetType().ToString().Contains("Codeunit"))
        {
            if (context.ContainingSymbol.GetContainingObjectTypeSymbol().GetProperty(PropertyKind.TableNo) != null)
            {
                return true;
            }
        }

        return false;
    }
}