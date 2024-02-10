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

        private static readonly List<string> databaseInvocations = new List<string>
        {
            "insert", "delete", "modify", "modifyall", "rename", "addlink", "deletelink", "deletelinks", "commit"
        };

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.FindDatabaseReadInTryFunctions), SyntaxKind.InvocationExpression);

        private void FindDatabaseReadInTryFunctions(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.Kind != SymbolKind.Method) return;
            IMethodSymbol symbol = (IMethodSymbol)ctx.ContainingSymbol;

            if (!symbol.Attributes.Any(a => a.AttributeKind == AttributeKind.TryFunction)) return;

            InvocationExpressionSyntax invocation = ctx.Node as InvocationExpressionSyntax;
            if (invocation == null) return;

            MemberAccessExpressionSyntax expression = invocation.Expression as MemberAccessExpressionSyntax;
            if (expression == null) return;

            if (databaseInvocations.Contains(expression.Name.Identifier.ValueText.ToLowerInvariant()))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0054DatabaseReadInTryFunctions, ctx.Node.GetLocation()));
            }
        }
    }
}