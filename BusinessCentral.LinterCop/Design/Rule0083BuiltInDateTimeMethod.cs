#if !LessThenFall2024
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0083BuiltInDateTimeMethod : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod);

    public override VersionCompatibility SupportedVersions => VersionCompatibility.Fall2024OrGreater;

    public override void Initialize(AnalysisContext context) =>
          context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), OperationKind.InvocationExpression);

    private void AnalyzeInvocation(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.Arguments.Length < 1)
            return;

        string? recommendedMethod = operation.TargetMethod.Name switch
        {
            "Date2DMY" => GetDate2DMYReplacement(operation),
            "Date2DWY" => GetDate2DWYReplacement(operation),
            "DT2Date" => "Date",
            "DT2Time" => "Time",
            "Format" => GetFormatReplacement(operation),
            _ => null
        };

        if (string.IsNullOrEmpty(recommendedMethod))
            return;

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod,
            ctx.Operation.Syntax.GetLocation(),
            operation.Arguments[0].Value.Syntax.ToString().QuoteIdentifierIfNeeded(),
            recommendedMethod));
    }

    private string? GetDate2DMYReplacement(IInvocationExpression operation)
    {
        if (operation.Arguments.Length < 2)
            return null;

        return operation.Arguments[1].Value.ConstantValue.Value switch
        {
            1 => "Day",
            2 => "Month",
            3 => "Year",
            _ => "<Day/Month/Year>"
        };
    }

    private string? GetDate2DWYReplacement(IInvocationExpression operation)
    {
        int formatSpecifier = -1;

        if (operation.Arguments.Length >= 2 && operation.Arguments[1].Value.ConstantValue.Value is int extractedValue)
            formatSpecifier = extractedValue;

        return formatSpecifier switch
        {
            1 => "DayOfWeek",
            2 => "Year",
            _ => "<DayOfWeek/Year>"
        };
    }

    private string? GetFormatReplacement(IInvocationExpression operation)
    {
        if (operation.Arguments.Length < 3)
            return string.Empty;

        return  operation.Arguments[2].Value.ConstantValue.Value?.ToString() switch
        {
            "<HOURS24>" => "Hour",
            "<MINUTES>" => "Minute",
            "<SECONDS>" => "Second",
            "<THOUSANDS>" => "Millisecond",
            _ => string.Empty
        };
    }
}
#endif