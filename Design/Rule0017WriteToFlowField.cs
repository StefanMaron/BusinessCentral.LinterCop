using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Semantics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    class Rule0017WriteToFlowField : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0017WriteToFlowField, DiagnosticDescriptors.Rule0000ErrorInRule);

        public override void Initialize(AnalysisContext context)
            => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckForWriteToFlowField),
                OperationKind.AssignmentStatement,
                OperationKind.InvocationExpression
            );

        private void CheckForWriteToFlowField(OperationAnalysisContext context)
        {
            if (context.ContainingSymbol.IsObsoletePending || context.ContainingSymbol.IsObsoleteRemoved) return;
            if (context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;

            if (context.Operation.Kind == OperationKind.InvocationExpression)
            {
                try
                {
                    IInvocationExpression operation = (IInvocationExpression)context.Operation;
                    if (operation.TargetMethod.Name == "Validate" && operation.TargetMethod.ContainingType.ToString() == "Table")
                    {
                        var FieldClass = ((IFieldAccess)((IConversionExpression)operation.Arguments[0].Value).Operand).FieldSymbol.FieldClass;
                        if (FieldClass == FieldClassKind.FlowField)
                            if (!HasExplainingComment(context.Operation))
                                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0017WriteToFlowField, context.Operation.Syntax.GetLocation()));
                    }
                }
                catch (InvalidCastException)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0000ErrorInRule, context.Operation.Syntax.GetLocation(), new Object[] { "Rule0017", "InvalidCastException", "at Line 41" }));
                }
                catch (ArgumentException)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0000ErrorInRule, context.Operation.Syntax.GetLocation(), new Object[] { "Rule0017", "ArgumentException", "at Line 45" }));
                }
            }
            else
            {
                IAssignmentStatement operation = (IAssignmentStatement)context.Operation;
                if (operation.Target.Kind == OperationKind.FieldAccess)
                {
                    try
                    {
                        var FieldClass = FieldClassKind.Normal;

                        if (operation.Target.Syntax.Kind == SyntaxKind.ArrayIndexExpression)
                            FieldClass = ((IFieldAccess)((ITextIndexAccess)operation.Target).TextExpression).FieldSymbol.FieldClass;
                        else
                            FieldClass = ((IFieldAccess)operation.Target).FieldSymbol.FieldClass;

                        if (FieldClass == FieldClassKind.FlowField)
                            if (!HasExplainingComment(context.Operation))
                                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0017WriteToFlowField, context.Operation.Syntax.GetLocation()));
                    }
                    catch (InvalidCastException)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0000ErrorInRule, context.Operation.Syntax.GetLocation(), new Object[] { "Rule0017", "InvalidCastException", "at Line 62" }));
                    }
                    catch (ArgumentException)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0000ErrorInRule, context.Operation.Syntax.GetLocation(), new Object[] { "Rule0017", "ArgumentException", "at Line 66" }));
                    }
                }
            }
        }

        private bool HasExplainingComment(IOperation operation)
        {
            foreach (SyntaxTrivia trivia in operation.Syntax.Parent.GetLeadingTrivia())
            {
                if (trivia.IsKind(SyntaxKind.LineCommentTrivia))
                {
                    return true;
                }
            }
            foreach (SyntaxTrivia trivia in operation.Syntax.Parent.GetTrailingTrivia())
            {
                if (trivia.IsKind(SyntaxKind.LineCommentTrivia))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
