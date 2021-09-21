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
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForObjectIDsInVariablesOrProperties), new SymbolKind[]
            {
                SymbolKind.GlobalVariable,
                SymbolKind.LocalVariable,
                SymbolKind.Method
            });

        }

        private void CheckForObjectIDsInVariablesOrProperties(SymbolAnalysisContext ctx)
        {
            ISymbol symbol = (ISymbol)ctx.Symbol;
            string declariation = GetDeclaration(symbol);

            if (symbol.Kind == SymbolKind.GlobalVariable || symbol.Kind == SymbolKind.LocalVariable)
            {
                IVariableSymbol variable = (IVariableSymbol)symbol;
                if (IsObjectType(variable.Type.NavTypeKind))
                {
                    if (Regex.IsMatch(declariation, @"\d+"))
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Symbol.GetLocation(), new object[] { declariation }));
                    }
                }
            }
            else if (symbol.Kind == SymbolKind.Method)
            {
                IMethodSymbol method = (IMethodSymbol)symbol;

                foreach (IParameterSymbol member in method.Parameters)
                {
                    if (IsObjectType(member.ParameterType.NavTypeKind))
                    {
                        declariation = GetDeclaration(member);

                        if (Regex.IsMatch(declariation, @"\d+"))
                        {
                            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, member.GetLocation(), new object[] { declariation }));
                        }
                    }
                }

                if (IsObjectType(method.ReturnValueSymbol.ReturnType.NavTypeKind))
                {
                    declariation = GetDeclaration(method.ReturnValueSymbol);

                    if (Regex.IsMatch(declariation, @"\d+"))
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, method.ReturnValueSymbol.GetLocation(), new object[] { declariation }));
                    }
                }


            }
            else
            {
                if (Regex.IsMatch(declariation, @"\d+"))
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0003DoNotUseObjectIDsInVariablesOrProperties, ctx.Symbol.GetLocation(), new object[] { declariation }));
                }
            }
        }

        private static string GetDeclaration(ISymbol symbol)
        {
            return symbol.Location.SourceTree.GetText(CancellationToken.None).GetSubText(symbol.DeclaringSyntaxReference.Span).ToString();
        }

        private static bool IsObjectType(NavTypeKind type)
        {
            return (new NavTypeKind[] { NavTypeKind.Record, NavTypeKind.Codeunit, NavTypeKind.Enum, NavTypeKind.Page, NavTypeKind.Query, NavTypeKind.Report, NavTypeKind.TestPage, NavTypeKind.XmlPort }).Contains(type);
        }
    }
    
}
