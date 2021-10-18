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
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;

            var correctName = "";

            if (ctx.ContainingSymbol.Kind == SymbolKind.LocalVariable || ctx.ContainingSymbol.Kind == SymbolKind.GlobalVariable)
            {
                IVariableSymbol variable = (IVariableSymbol)ctx.ContainingSymbol;

                if (variable.Type.NavTypeKind == NavTypeKind.Array)
                    correctName = GetCorrectName(ctx.Node.Parent.Parent.ToString().Replace(" temporary","").Replace(ctx.Node.ToString(), "").Trim(), variable.Type.ToString());
                else
                    correctName = GetCorrectName(variable.Type.NavTypeKind.ToString(), variable.Type.ToString());
                
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

                if (property.PropertyKind != PropertyKind.Permissions && property.PropertyKind.ToString() != "AccessByPermission") //seems like there was a breaking change in some version of the compiler where internal ids moved. Using text to be save
                {
                    if (ctx.Node.ToString().Trim('"').ToUpper() != property.ValueText.ToUpper())
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Node.GetLocation(), new object[] { ctx.Node.ToString().Trim('"'), property.ValueText }));

                    if (ctx.Node.ToString().Trim('"') != property.ValueText)
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0005VariableCasingShouldNotDIfferFromDeclaration, ctx.Node.GetLocation(), new object[] { property.ValueText, "" }));
                }
            }

            if (ctx.ContainingSymbol.Kind == SymbolKind.Method)
            {
                IMethodSymbol method = (IMethodSymbol)ctx.ContainingSymbol;

                foreach (IParameterSymbol parameter in method.Parameters)
                {
                    if (ctx.Node.GetLocation().SourceSpan.End == parameter.DeclaringSyntaxReference.GetSyntax(CancellationToken.None).Span.End)
                    {
                        correctName = GetCorrectName(parameter.ParameterType.NavTypeKind.ToString(), parameter.ParameterType.ToString());

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
                        correctName = GetCorrectName(returnValue.ReturnType.NavTypeKind.ToString(), returnValue.ReturnType.ToString());

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

        private static string GetCorrectName(string kind, string OldName)
        {
            if (OldName.Trim().StartsWith(kind))
            {
                OldName = OldName.Substring(kind.Length + 1).Trim(' ', '"');
            }

            return OldName;
        }
    }    
}
