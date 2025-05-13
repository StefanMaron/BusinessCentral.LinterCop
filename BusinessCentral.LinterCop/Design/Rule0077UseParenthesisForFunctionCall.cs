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

        // Exclude using methodes like IsolationLevel::UpdLock and/or TextEncoding::Windows
        if (ctx.Operation.Syntax.Parent.IsKind(SyntaxKind.OptionAccessExpression))
            return;

        if (!operation.Syntax.GetLastToken().IsKind(SyntaxKind.CloseParenToken))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0077UseParenthesisForFunctionCall,
                operation.Syntax.GetLocation(),
                method.Name));
        }
    }
}