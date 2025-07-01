#if !LessThenFall2024
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0083BuiltInDateTimeMethod : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod);

    public override VersionCompatibility SupportedVersions =>
        VersionCompatibility.Fall2024OrGreater;

    public override void Initialize(AnalysisContext context) =>
          context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), OperationKind.InvocationExpression);

    private void AnalyzeInvocation(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.Arguments.Length < 1)
            return;

        if (operation.Syntax is not InvocationExpressionSyntax invocationExpression)
            return;

        string? recommendedMethod = Rule0083BuiltInDateTimeMethodHelper.GetReplacementMethod(operation.TargetMethod.Name, invocationExpression)?.ToString();
        if (string.IsNullOrEmpty(recommendedMethod))
            return;

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod,
            invocationExpression.GetLocation(),
            operation.Arguments[0].Value.Syntax.ToString(),
            recommendedMethod));
    }
}
#endif