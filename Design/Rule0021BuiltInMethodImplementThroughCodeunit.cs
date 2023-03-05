using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0021BuiltInMethodImplementThroughCodeunit : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(
            DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement,
            DiagnosticDescriptors.Rule0022GlobalLanguageImplementTranslationHelper,
            DiagnosticDescriptors.Rule0027RunPageImplementPageManagement,
            DiagnosticDescriptors.Rule0000ErrorInRule);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckBuiltInMethod), OperationKind.InvocationExpression);

        private void CheckBuiltInMethod(OperationAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            if (operation.Arguments.Count() > 1 && operation.TargetMethod.ContainingType.NavTypeKind == NavTypeKind.Page)
            {
                IReturnValueSymbol returnValue = operation.TargetMethod.ReturnValueSymbol;
                if (returnValue.ReturnType.NavTypeKind == NavTypeKind.Action) return;               // Page Management Codeunit doesn't support returntype Action
                if (operation.TargetMethod.Name.ToUpper() == "ENQUEUEBACKGROUNDTASK") return;       // do not execute on CurrPage.EnqueueBackgroundTask

                if (operation.TargetMethod.Parameters[0].ParameterType.NavTypeKind == NavTypeKind.Integer)
                {
                    if (operation.Arguments[0].Syntax.ToString().ToUpper().Substring(0, 6) != "PAGE::") return; // In case the PageID is set by a field from a (setup) record
                }
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0027RunPageImplementPageManagement, ctx.Operation.Syntax.GetLocation()));
                return;
            }

            switch (operation.TargetMethod.Name.ToUpper())
            {
                case "CONFIRM":
                    try
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0021ConfirmImplementConfirmManagement, ctx.Operation.Syntax.GetLocation()));
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0000ErrorInRule, ctx.Operation.Syntax.GetLocation(), new Object[] { "Rule0021", "ArgumentOutOfRangeException", "at Line 29" }));
                    }
                    break;
                case "GLOBALLANGUAGE":
                    try
                    {
                        if (operation.Arguments.Length != 0) { ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0022GlobalLanguageImplementTranslationHelper, ctx.Operation.Syntax.GetLocation())); }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0000ErrorInRule, ctx.Operation.Syntax.GetLocation(), new Object[] { "Rule0022", "ArgumentOutOfRangeException", "at Line 39" }));
                    }
                    break;
            }
        }
    }
}