using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0003DoNotUseObjectIDsInVariablesOrProperties : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.CheckForObjectIDsInVariablesOrProperties), SyntaxKind.ObjectReference);

        }
        private void CheckForObjectIDsInVariablesOrProperties(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.Kind == SymbolKind.LocalVariable || ctx.ContainingSymbol.Kind == SymbolKind.GlobalVariable)
            {
                IVariableSymbol variable = (IVariableSymbol)ctx.ContainingSymbol;
                if (ctx.Node.ToString().Trim('"') != variable.Type.ToString().Replace(variable.Type.NavTypeKind.ToString(), "").Trim(' ', '"'))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), variable.Type.ToString().Replace(variable.Type.NavTypeKind.ToString(),"").Trim(' ','"') }));
            }
            if (ctx.ContainingSymbol.Kind == SymbolKind.Property)
            {
                IPropertySymbol property = (IPropertySymbol)ctx.ContainingSymbol;
                if (ctx.Node.ToString().Trim('"') != property.ValueText)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), property.ValueText }));
            }

            if (ctx.ContainingSymbol.Kind == SymbolKind.Method)
            {
                IMethodSymbol method = (IMethodSymbol)ctx.ContainingSymbol;

                foreach (IParameterSymbol parameter in method.Parameters)
                {
                    if (ctx.Node.GetLocation().SourceSpan.End == parameter.DeclaringSyntaxReference.GetSyntax(CancellationToken.None).Span.End)
                    {
                        if (ctx.Node.ToString().Trim('"') != parameter.ParameterType.ToString().Replace(parameter.ParameterType.NavTypeKind.ToString(), "").Trim(' ', '"'))
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), parameter.ParameterType.ToString().Replace(parameter.ParameterType.NavTypeKind.ToString(), "").Trim(' ', '"') }));
                    }                    
                }
                try
                {
                    IReturnValueSymbol returnValue = (IReturnValueSymbol)method.ReturnValueSymbol;

                    if (ctx.Node.GetLocation().SourceSpan.End == returnValue.DeclaringSyntaxReference.GetSyntax(CancellationToken.None).Span.End)
                    {
                        if (ctx.Node.ToString().Trim('"') != returnValue.ReturnType.ToString().Replace(returnValue.ReturnType.NavTypeKind.ToString(), "").Trim(' ', '"'))
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), returnValue.ReturnType.ToString().Replace(returnValue.ReturnType.NavTypeKind.ToString(), "").Trim(' ', '"') }));
                    }
                }
                catch (System.NullReferenceException)
                { }
            }
        }
    }    
}
