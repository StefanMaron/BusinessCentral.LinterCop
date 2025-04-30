using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Semantics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0071DoNotSetIsHandledToFalse : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0071DoNotSetIsHandledToFalse);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckIsHandledAssignments), OperationKind.AssignmentStatement);
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckIsHandledInvocations), OperationKind.InvocationExpression);
    }

    private void CheckIsHandledInvocations(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression invocation)
            return;

        // Ensure TargetMethod has valid parameters
        if (invocation.TargetMethod.Parameters.Length != invocation.Arguments.Length)
            return;

        for (int i = 0; i < invocation.Arguments.Length; i++)
        {
            var argument = invocation.Arguments[i];
            var parameter = invocation.TargetMethod.Parameters[i];

            // Check if argument is a reference to IsHandled passed as a var parameter
            if (argument.Value is not IParameterReferenceExpression parameterRef)
                return;

            if (!parameter.IsVar)
                return;

            if (!IsIsHandledEventSubscriberParameter(parameterRef.Parameter))
                return;

            if (HasPrecedingExitStatement(ctx.Operation, parameterRef.Parameter))
                return;

            // Report the diagnostic
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0071DoNotSetIsHandledToFalse,
                argument.Value.Syntax.GetLocation()));
        }
    }

    private void CheckIsHandledAssignments(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Operation is not IAssignmentStatement assignment)
            return;

        if (assignment.Target.Kind != OperationKind.ParameterReferenceExpression)
            return; // check the parameter is assigned a value

        IParameterSymbol parameter = ((IParameterReferenceExpression)assignment.Target).Parameter;

        if (!IsIsHandledEventSubscriberParameter(parameter))
            return;

        if (assignment.Value.ConstantValue.HasValue && (bool)assignment.Value.ConstantValue.Value)
            return; // check for true assignment

        if (HasPrecedingExitStatement(ctx.Operation, parameter))
            return;

        if (IsSelfGuardedOrAssignment(assignment.Value))
            return;

        // any other not true assignment should not be done
        ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0071DoNotSetIsHandledToFalse,
                assignment.Syntax.GetLocation()));
    }

    private bool IsIsHandledEventSubscriberParameter(IParameterSymbol parameter)
    {
        if (parameter.ContainingSymbol is null)
            return false;
        // check for event subscriber method
        if (parameter.ContainingSymbol.Kind != SymbolKind.Method)
            return false;
        if (!((IMethodSymbol)parameter.ContainingSymbol).IsEventSubscriber())
            return false;

        // check for "var IsHandled: Boolean" parameter
        if (!CheckIsHandledName(parameter.Name) || (parameter.ParameterType.NavTypeKind != NavTypeKind.Boolean) || !parameter.IsVar)
            return false;

        return true;
    }

    private bool CheckIsHandledName(string name)
    {
        // checks for name(s) used with the "IsHandled Pattern"
        // "Handled" is also used in the Base / System App, see: https://github.com/search?q=repo%3AStefanMaron%2FMSDyn365BC.Code.History+%22var+Handled%3A+Boolean%22&type=code
        return SemanticFacts.IsSameName(name, "ishandled") || SemanticFacts.IsSameName(name, "handled");
    }

    private static bool HasPrecedingExitStatement(IOperation operation, IParameterSymbol parameter)
    {
        var parent = operation.Syntax.Parent;
        while (parent.Kind != SyntaxKind.Block && parent.Kind != SyntaxKind.None)
        {
            parent = parent.Parent;
        }

        IEnumerable<SyntaxNode> identifiers = parent.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Where(n => n.Identifier.ValueText == parameter.Name && n.SpanStart < operation.Syntax.SpanStart);

        foreach (var identifier in identifiers)
        {
            if (identifier.Parent is IfStatementSyntax ifStatement)
            {
                // Check if the condition is the identifier "IsHandled"
                if (ifStatement.Condition is IdentifierNameSyntax condition &&
                    condition.Identifier.ValueText == parameter.Name)
                {
                    // Check if the body is a single "exit;" statement
                    if (ifStatement.Statement is ExitStatementSyntax)
                    {
                        // Valid "if IsHandled then exit;" detected
                        return true;
                    }
                }
            }
        }

        // No valid preceding exit statement was found
        return false;
    }

    private bool IsSelfGuardedOrAssignment(IOperation operation)
    {
        // checks for the pattern: IsHandled := IsHandled or SomeOtherCondition;

        if (operation.Kind != OperationKind.BinaryOperatorExpression)
            return false;
        IBinaryOperatorExpression operatorExpression = (IBinaryOperatorExpression)operation;
        if (operatorExpression.BinaryOperationKind is not (BinaryOperationKind.BooleanOr or BinaryOperationKind.BooleanConditionalOr))
            return false;

        // check left side of or statement
        if (operatorExpression.LeftOperand.Kind == OperationKind.ParameterReferenceExpression)
        {
            IParameterSymbol parameter = ((IParameterReferenceExpression)operatorExpression.LeftOperand).Parameter;
            if (IsIsHandledEventSubscriberParameter(parameter))
                return true;
        }
        // check right side of or statement
        if (operatorExpression.RightOperand.Kind == OperationKind.ParameterReferenceExpression)
        {
            IParameterSymbol parameter = ((IParameterReferenceExpression)operatorExpression.RightOperand).Parameter;
            if (IsIsHandledEventSubscriberParameter(parameter))
                return true;
        }

        return false;
    }
}
