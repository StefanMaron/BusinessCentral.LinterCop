using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

// This rule is not finished 100% yet.
// It does false warning if the a function is called like one of the database functions, but it is not a database function.
// it should also analyze the references recursively to see if the current statement is inside a try function or not.

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0054DatabaseReadInTryFunctions : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0054DatabaseReadInTryFunctions);
        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.FindDatabaseReadInTryFunctions), SyntaxKind.InvocationExpression);

        private void FindDatabaseReadInTryFunctions(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.Kind != SymbolKind.Method) return;

            IMethodSymbol symbol = (IMethodSymbol)ctx.ContainingSymbol;
            if (symbol.Attributes.Any(a => a.AttributeKind == AttributeKind.TryFunction))
            {
                String[] databaseInvocations = { "INSERT", "DELETE", "MODIFY", "MODIFYALL", "RENAME", "ADDLINK", "DELETELINK", "DELETELINKS", "COMMIT" };
                string currentInvocation = ((IdentifierNameSyntax)((InvocationExpressionSyntax)ctx.Node).Expression).Identifier.ValueText;
                if (databaseInvocations.Contains(currentInvocation.ToUpper()))
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0054DatabaseReadInTryFunctions, ctx.Node.GetLocation()));
                }
            }
        }
    }
}