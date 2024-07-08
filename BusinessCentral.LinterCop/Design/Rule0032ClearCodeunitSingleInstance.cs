using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0032ClearCodeunitSingleInstance : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0032ClearCodeunitSingleInstance, DiagnosticDescriptors.Rule0000ErrorInRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.ClearCodeunit), OperationKind.InvocationExpression);
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.ClearAllCodeunit), OperationKind.InvocationExpression);
        }

        private void ClearCodeunit(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "Clear")) return;
            if (operation.Arguments.Count() < 1) return;

            if (operation.Arguments[0].Value is not IConversionExpression boundConversion)
                return;

            IOperation operand = boundConversion.Operand;
            if (operand.GetSymbol().GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Codeunit) return;

            if (IsSingleInstanceCodeunitWithGlobalVars((ICodeunitTypeSymbol)operand.GetSymbol().GetTypeSymbol()))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0032ClearCodeunitSingleInstance, ctx.Operation.Syntax.GetLocation(), new Object[] { operand.GetSymbol().Name, operand.GetSymbol().GetTypeSymbol().Name }));
        }

        private void ClearAllCodeunit(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Codeunit) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;
            if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "ClearAll")) return;

            IEnumerable<ISymbol> localVariables = ((IMethodSymbol)ctx.ContainingSymbol.OriginalDefinition).LocalVariables
                                                            .Where(var => var.OriginalDefinition.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.Codeunit)
                                                            .Where(var => var.OriginalDefinition.GetTypeSymbol().OriginalDefinition != ctx.ContainingSymbol.GetContainingObjectTypeSymbol().OriginalDefinition);
            IEnumerable<ISymbol> globalVariables = ctx.ContainingSymbol.GetContainingObjectTypeSymbol()
                                                            .GetMembers()
                                                            .Where(members => members.Kind == SymbolKind.GlobalVariable)
                                                            .Where(var => var.OriginalDefinition.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.Codeunit)
                                                            .Where(var => var.OriginalDefinition.GetTypeSymbol().OriginalDefinition != ctx.ContainingSymbol.GetContainingObjectTypeSymbol().OriginalDefinition);

            if (HasSingleInstanceCodeunitWithGlobalVars(localVariables, out ISymbol codeunit) || HasSingleInstanceCodeunitWithGlobalVars(globalVariables, out codeunit))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0032ClearCodeunitSingleInstance, ctx.Operation.Syntax.GetLocation(), new Object[] { codeunit.Name, codeunit.GetTypeSymbol().Name }));
        }

        private static bool HasSingleInstanceCodeunitWithGlobalVars(IEnumerable<ISymbol> variables, out ISymbol codeunit)
        {
            foreach (ISymbol variable in variables.Where(var => var.OriginalDefinition.ContainingType.GetNavTypeKindSafe() == NavTypeKind.Codeunit))
                if (IsSingleInstanceCodeunitWithGlobalVars((ICodeunitTypeSymbol)variable.OriginalDefinition.GetTypeSymbol()))
                {
                    codeunit = variable;
                    return true;
                }

            codeunit = null;
            return false;
        }

        private static bool IsSingleInstanceCodeunitWithGlobalVars(ICodeunitTypeSymbol codeunitTypeSymbol)
        {
            IPropertySymbol singleInstanceProperty = codeunitTypeSymbol.GetProperty(PropertyKind.SingleInstance);
            if (singleInstanceProperty == null || !(bool)singleInstanceProperty.Value) return false;

            var globalVariables = codeunitTypeSymbol.GetMembers().Where(members => members.Kind == SymbolKind.GlobalVariable);
            var globalVariablesNonRecordTypes = globalVariables.Where(vars => vars.GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Record);

            bool globalVariablesExists = globalVariablesNonRecordTypes.Count() != 0;
            return globalVariablesExists;
        }
    }
}