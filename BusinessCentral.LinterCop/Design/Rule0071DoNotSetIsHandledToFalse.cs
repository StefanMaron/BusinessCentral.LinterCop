using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Semantics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0071DoNotSetIsHandledToFalse : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0071DoNotSetIsHandledToFalse);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckIsHandledAssignments), OperationKind.AssignmentStatement);
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckIsHandledInvocations), OperationKind.InvocationExpression);
        }

        private void CheckIsHandledInvocations(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            var invocation = (IInvocationExpression)ctx.Operation;
            for (int i = 0; i < invocation.Arguments.Length; i++)
            {
                // check if IsHandled is passed to another method with var
                var invocationValue = invocation.Arguments[i].Value;
                if (invocationValue.Kind == OperationKind.ParameterReferenceExpression)
                    if (IsIsHandledEventSubscriberParameter(((IParameterReferenceExpression)invocationValue).Parameter))
                        if (invocation.TargetMethod.Parameters[i].IsVar)
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0071DoNotSetIsHandledToFalse, invocationValue.Syntax.GetLocation()));
            }
        }

        private void CheckIsHandledAssignments(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            var assignment = (IAssignmentStatement)ctx.Operation;
            if (assignment.Target.Kind != OperationKind.ParameterReferenceExpression)
                return; // check the parameter is assigned a value

            if (!IsIsHandledEventSubscriberParameter(((IParameterReferenceExpression)assignment.Target).Parameter))
                return;

            if (assignment.Value.ConstantValue.HasValue)
                if ((bool)assignment.Value.ConstantValue.Value)
                    return; // check for true assignment

            // any other not true assignment should not be done
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0071DoNotSetIsHandledToFalse, assignment.Syntax.GetLocation()));
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
            return (name.ToLower() == "ishandled") || (name.ToLower() == "handled");
        }
    }
}
