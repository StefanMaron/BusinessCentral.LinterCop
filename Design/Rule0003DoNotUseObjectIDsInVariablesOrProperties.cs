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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties,DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.CheckForObjectIDsInVariablesOrProperties), new SyntaxKind[] {
                SyntaxKind.ObjectReference,
                SyntaxKind.PermissionValue
            });

        }
        private void CheckForObjectIDsInVariablesOrProperties(SyntaxNodeAnalysisContext ctx)
        {
            var correctName = "";

            if (ctx.ContainingSymbol.Kind == SymbolKind.LocalVariable || ctx.ContainingSymbol.Kind == SymbolKind.GlobalVariable)
            {
                IVariableSymbol variable = (IVariableSymbol)ctx.ContainingSymbol;
                correctName = GetCorrectName(variable.Type.NavTypeKind, variable.Type.ToString());

                if (ctx.Node.ToString().Trim('"').ToUpper() != correctName.ToUpper())
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), correctName }));

                if (ctx.Node.ToString().Trim('"') != correctName)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Node.GetLocation(), new object[] { correctName, "" }));
            }
            if (ctx.ContainingSymbol.Kind == SymbolKind.Property)
            {
                IPropertySymbol property = (IPropertySymbol)ctx.ContainingSymbol;

                if (ctx.Node.Kind == SyntaxKind.PermissionValue)
                {
                    var nodes = ctx.Node.ChildNodesAndTokens().GetEnumerator();
                   
                    while (nodes.MoveNext())
                    {
                        if (nodes.Current.IsNode)
                        {
                            var subnodes = nodes.Current.ChildNodesAndTokens().GetEnumerator();

                            while (subnodes.MoveNext())
                            {                                
                                if (subnodes.Current.Kind == SyntaxKind.ObjectId)
                                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, nodes.Current.GetLocation(), new object[] { "", "the object name" }));
                            };
                        }

                    };
                }

                if (ctx.Node.ToString().Trim('"').ToUpper() != property.ValueText.ToUpper() && property.PropertyKind != PropertyKind.Permissions && property.PropertyKind != PropertyKind.AccessByPermission)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), property.ValueText }));

                if (ctx.Node.ToString().Trim('"') != property.ValueText && property.PropertyKind != PropertyKind.Permissions && property.PropertyKind != PropertyKind.AccessByPermission)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Node.GetLocation(), new object[] {  property.ValueText,"" }));
            }

            if (ctx.ContainingSymbol.Kind == SymbolKind.Method)
            {
                IMethodSymbol method = (IMethodSymbol)ctx.ContainingSymbol;

                foreach (IParameterSymbol parameter in method.Parameters)
                {
                    if (ctx.Node.GetLocation().SourceSpan.End == parameter.DeclaringSyntaxReference.GetSyntax(CancellationToken.None).Span.End)
                    {
                        correctName = GetCorrectName(parameter.ParameterType.NavTypeKind, parameter.ParameterType.ToString());

                        if (ctx.Node.ToString().Trim('"').ToUpper() != correctName.ToUpper())
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), correctName }));

                        if (ctx.Node.ToString().Trim('"') != correctName)
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Node.GetLocation(), new object[] { correctName, "" }));
                    }                    
                }
                try
                {
                    IReturnValueSymbol returnValue = (IReturnValueSymbol)method.ReturnValueSymbol;

                    if (ctx.Node.GetLocation().SourceSpan.End == returnValue.DeclaringSyntaxReference.GetSyntax(CancellationToken.None).Span.End)
                    {
                        correctName = GetCorrectName(returnValue.ReturnType.NavTypeKind, returnValue.ReturnType.ToString());

                        if (ctx.Node.ToString().Trim('"').ToUpper() != correctName.ToUpper())
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), correctName }));

                        if (ctx.Node.ToString().Trim('"') != correctName)
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Node.GetLocation(), new object[] { correctName,"" }));
                    }
                }
                catch (System.NullReferenceException)
                { }
            }
        }

        private static string GetCorrectName(NavTypeKind kind, string OldName)
        {
            if (OldName.StartsWith(kind.ToString()))
            {
                OldName = OldName.Substring(kind.ToString().Length + 1).Trim(' ', '"');
            }

            return OldName;
        }
    }    
}
