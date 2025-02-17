using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0077MissingParenthesis : DiagnosticAnalyzer
{
    private static readonly HashSet<string> MethodsRequiringParenthesis = [
        "CurrentDateTime",
        "CompanyName",
        "Count",
        "GetLastErrorCallStack",
        "GetLastErrorCode",
        "GuiAllowed",
        "HasCollectedErrors",
        "IsEmpty",
        "Time",
        "Today",
        "UserId",
        "WorkDate"
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0077MissingParenthesis);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(AnalyzeParenthesis, OperationKind.InvocationExpression);

    private void AnalyzeParenthesis(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Operation is not IInvocationExpression { Arguments.Length: 0 } operation ||
            operation.TargetMethod is not IMethodSymbol { MethodKind: MethodKind.BuiltInMethod } method)
            return;

        if (MethodsRequiringParenthesis.Contains(method.Name) &&
            !operation.Syntax.GetLastToken().IsKind(SyntaxKind.CloseParenToken))
        {
            var location = operation.Syntax.GetIdentifierNameSyntax()?.GetLocation() ?? operation.Syntax.GetLocation();
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0077MissingParenthesis,
                location,
                method.Name));
        }
    }
}