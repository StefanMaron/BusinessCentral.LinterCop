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
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.CheckForVariableWithCasingMismatch), SyntaxKind.IdentifierName);
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckForBuiltInMethodsWithCasingMismatch), OperationKind.InvocationExpression);
        }

        private void CheckForBuiltInMethodsWithCasingMismatch(OperationAnalysisContext ctx)
        {
            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;

            var nodes = Array.Find(operation.Syntax.DescendantNodes().ToArray(), element => element.ToString().ToUpper() == operation.TargetMethod.Name.ToString().ToUpper() && element.ToString() != operation.TargetMethod.Name.ToString());
            if (nodes != null)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Operation.Syntax.GetLocation(), new object[] { operation.TargetMethod.Name, "" }));
        }
        private void CheckForVariableWithCasingMismatch(SyntaxNodeAnalysisContext ctx)
        {
            List<ISymbol> vars = new List<ISymbol>();

            if (ctx.ContainingSymbol.Kind != SymbolKind.Method)
            {
                return;
            }
            IMethodSymbol method = (IMethodSymbol)ctx.ContainingSymbol;
            if (method.IsEvent || method.IsObsoleteRemoved || method.IsObsoletePending)
            {
                return;
            }
            IContainerSymbol currObject = (IContainerSymbol)method.ContainingSymbol;
            while (currObject.Kind == SymbolKind.Field || currObject.Kind == SymbolKind.Control)
                currObject = (IContainerSymbol)currObject.ContainingSymbol;

            foreach (ISymbol symbol in currObject.GetMembers())
            {
                if (symbol.Kind == SymbolKind.GlobalVariable)
                    vars.Add(symbol);
            }
            foreach (ISymbol symbol in method.LocalVariables)
            {
                    vars.Add(symbol);
            }
            foreach (ISymbol symbol in method.Parameters)
            {
                    vars.Add(symbol);
            }
            vars.Add(method.ReturnValueSymbol);

            foreach (ISymbol variable in vars)
            {
                if (variable != null)
                    if (ctx.Node.ToString().ToUpper() == variable.Name.ToUpper() && ctx.Node.ToString() != variable.Name.ToString())
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Node.GetLocation(), new object[] { variable.Name ,"" }));
            }
            
    }
  }
}
