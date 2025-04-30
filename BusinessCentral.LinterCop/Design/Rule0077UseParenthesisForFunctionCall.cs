using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0077UseParenthesisForFunctionCall : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0077UseParenthesisForFunctionCall);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(AnalyzeInvocationExpression, OperationKind.InvocationExpression);

    private void AnalyzeInvocationExpression(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Operation is not IInvocationExpression { Arguments.Length: 0 } operation ||
            operation.TargetMethod is not IMethodSymbol { MethodKind: MethodKind.BuiltInMethod } method)
            return;

        // The CodeFixProvider for this rule only support "CurrentDateTime();" and not "System.CurrentDateTime();"
        // So for now, only raise a diagnostic where we also can provide a code fix.
        if (ctx.Operation.Syntax is MemberAccessExpressionSyntax)
            return;

        if (!operation.Syntax.GetLastToken().IsKind(SyntaxKind.CloseParenToken))
        {
            var location = operation.Syntax.GetIdentifierNameSyntax()?.GetLocation() ?? operation.Syntax.GetLocation();
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0077UseParenthesisForFunctionCall,
                location,
                method.Name));
        }
    }
}