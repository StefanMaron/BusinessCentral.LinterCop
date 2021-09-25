using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeCop.Utilities;
using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0005VariableCasingShouldNotDIfferFromDeclaration : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckForBuiltInMethodsWithCasingMismatch), new OperationKind[] { 
                OperationKind.InvocationExpression,
                OperationKind.FieldAccess,
                OperationKind.GlobalReferenceExpression,
                OperationKind.LocalReferenceExpression,
                OperationKind.ParameterReferenceExpression,
                OperationKind.ReturnValueReferenceExpression
            });
        }

        private void CheckForBuiltInMethodsWithCasingMismatch(OperationAnalysisContext ctx)
        {
            var targetName = "";
            if (ctx.Operation.Kind == OperationKind.InvocationExpression)
            {
                IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
                targetName = operation.TargetMethod.Name;
            }
            if (ctx.Operation.Kind == OperationKind.FieldAccess)
            {
                IFieldAccess operation = (IFieldAccess)ctx.Operation;
                targetName = operation.FieldSymbol.Name;
            }
            if ( new object[] {
                OperationKind.GlobalReferenceExpression,
                OperationKind.LocalReferenceExpression,
                OperationKind.ParameterReferenceExpression,
                OperationKind.ReturnValueReferenceExpression }.Contains(ctx.Operation.Kind))
            {
                switch (ctx.Operation.Kind)
                {
                    case OperationKind.GlobalReferenceExpression:
                        targetName = ((IGlobalReferenceExpression)ctx.Operation).GlobalVariable.Name;
                        break;
                    case OperationKind.LocalReferenceExpression:
                        targetName = ((ILocalReferenceExpression)ctx.Operation).LocalVariable.Name;
                        break;
                    case OperationKind.ParameterReferenceExpression:
                        targetName = ((IParameterReferenceExpression)ctx.Operation).Parameter.Name;
                        break;
                    case OperationKind.ReturnValueReferenceExpression:
                        targetName = ((IReturnValueReferenceExpression)ctx.Operation).ReturnValue.Name;
                        break;
                }
            }

            if (OnlyDiffersInCasing(ctx.Operation.Syntax.ToString(), targetName))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Operation.Syntax.GetLocation(), new object[] { targetName, ctx.Operation.Syntax.Kind }));
                return;
            }

            var nodes = Array.Find(ctx.Operation.Syntax.DescendantNodes((SyntaxNode e) => true).ToArray(), element => OnlyDiffersInCasing(element.ToString(), targetName));
            if (nodes != null)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Operation.Syntax.GetLocation(), new object[] { targetName, "" }));
        }
        private bool OnlyDiffersInCasing(string left, string right)
        {
            return left.ToUpper() == right.ToUpper() && left != right;
        }

  }
}
