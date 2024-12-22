using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0008NoFilterOperatorsInSetRange : DiagnosticAnalyzer
{
    private readonly Lazy<Regex> replacementFieldPatternLazy = new Lazy<Regex>((Func<Regex>)(() => new Regex(@"%\d+", RegexOptions.Compiled)));

    private Regex ReplacementFieldPatternLazy => this.replacementFieldPatternLazy.Value;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0008NoFilterOperatorsInSetRange);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), OperationKind.InvocationExpression);

    private void AnalyzeInvocation(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.TargetMethod.Name != "SetRange" ||
            operation.TargetMethod.ContainingSymbol?.Name != "Table" ||
            operation.Arguments.Length < 2)
            return;

        CheckParameter(operation.Arguments[1].Value, ref operation, ref ctx);

        if (operation.Arguments.Length == 3)
            CheckParameter(operation.Arguments[2].Value, ref operation, ref ctx);
    }

    private void CheckParameter(IOperation operand, ref IInvocationExpression operation, ref OperationAnalysisContext context)
    {
        if (operand.Type.GetNavTypeKindSafe() != NavTypeKind.String && operand.Type.GetNavTypeKindSafe() != NavTypeKind.Joker)
            return;

        if (operand.Syntax.Kind != SyntaxKind.LiteralExpression)
            return;

        string parameterString = operand.Syntax.ToString();

        if (ContainsFilterOperators(parameterString))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0008NoFilterOperatorsInSetRange,
                operation.Syntax.GetLocation()));

            return;
        }

        Match match = this.ReplacementFieldPatternLazy.Match(parameterString);
        if (match.Success)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0008NoFilterOperatorsInSetRange,
                operation.Syntax.GetLocation()));
        }
    }

    private static bool ContainsFilterOperators(string parameterString) =>
        parameterString.IndexOfAny(new[] { '<', '>', '.', '*', '&', '|' }) >= 0 || parameterString.Contains("..");
}