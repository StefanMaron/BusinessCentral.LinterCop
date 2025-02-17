using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0050OperatorAndPlaceholderInFilterExpression : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0050OperatorAndPlaceholderInFilterExpression, DiagnosticDescriptors.Rule0059SingleQuoteEscapingIssueDetected);

    private static readonly string InvalidUnaryEqualsFilter = "'<>'''";

    public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), OperationKind.InvocationExpression);

    private void AnalyzeInvocation(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.TargetMethod.Name != "SetFilter" ||
            operation.Arguments.Length < 2)
            return;

        if (operation.Arguments[1].Value is not IOperation operand)
            return;

        if (operand.Type.GetNavTypeKindSafe() != NavTypeKind.String && operand.Type.GetNavTypeKindSafe() != NavTypeKind.Joker)
            return;

        if (operand.Syntax.Kind != SyntaxKind.LiteralExpression)
            return;

        string parameterString = operand.Syntax.ToFullString();
        if (parameterString.Equals(InvalidUnaryEqualsFilter))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0059SingleQuoteEscapingIssueDetected, operand.Syntax.GetLocation()));
            return;
        }

        string pattern = @"%\d+"; // Only when a placeholders (%1) is used in the filter expression we need to raise the rule that the placeholders won't work as expected
        Regex regex = new Regex(pattern);
        Match match = regex.Match(parameterString);
        if (!match.Success)
            return;

        int operatorIndex = parameterString.IndexOfAny("*?@".ToCharArray()); // Only the *, ? and @ operator changes the behavior of the placeholder
        if (operatorIndex == -1)
            return;

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0050OperatorAndPlaceholderInFilterExpression,
            operation.Syntax.GetLocation(),
            parameterString.Substring(operatorIndex, 1),
            match.Value));
    }
}