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
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "Clear")) return;
            if (operation.Arguments.Count() < 1) return;

            IOperation operand = ((IConversionExpression)operation.Arguments[0].Value).Operand;
            if (operand.GetSymbol().GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Codeunit) return;

            try // temporary add an Try/Catch to investigate issue https://github.com/StefanMaron/BusinessCentral.LinterCop/issues/523
            {
                if (IsSingleInstanceCodeunitWithGlobalVars((ICodeunitTypeSymbol)operand.GetSymbol().GetTypeSymbol()))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0032ClearCodeunitSingleInstance, ctx.Operation.Syntax.GetLocation(), new Object[] { operand.GetSymbol().Name, operand.GetSymbol().GetTypeSymbol().Name }));
            }
            catch
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0000ErrorInRule, ctx.Operation.Syntax.GetLocation(), new Object[] { "Rule0032", "Exception", "at Line 35" }));
            }
        }

        private void ClearAllCodeunit(OperationAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

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