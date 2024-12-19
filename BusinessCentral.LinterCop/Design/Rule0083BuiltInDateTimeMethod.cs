#if !LessThenFall2024
using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0083BuiltInDateTimeMethod : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0083BuiltInDateTimeMethod);

        public override VersionCompatibility SupportedVersions => VersionCompatibility.Fall2024OrGreater;

        public override void Initialize(AnalysisContext context) =>
              context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), OperationKind.InvocationExpression);

        private void AnalyzeInvocation(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved())
                return;

            if ((ctx.Operation is not IInvocationExpression operation) ||
                operation.TargetMethod is null ||
                operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
                operation.Arguments.Count() < 1)
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
                new object[] { operation.Arguments[0].Value.Syntax.ToString().QuoteIdentifierIfNeeded(), recommendedMethod }));
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

            if (operation.Arguments.Length >= 2 &&
                operation.Arguments[1].Value.ConstantValue.Value is int extractedValue)
            {
                formatSpecifier = extractedValue;
            }

            return formatSpecifier switch
            {
                1 => "DayOfWeek",
                2 => "Year",
                _ => "<DayOfWeek/Year>"
            };
        }

        private string? GetFormatReplacement(IInvocationExpression operation)
        {
            string? formatSpecifier = String.Empty;

            if (operation.Arguments.Length >= 3)
                formatSpecifier = operation.Arguments[2].Value.ConstantValue.Value?.ToString();

            return formatSpecifier switch
            {
                "<HOURS24>" => "Hour",
                "<MINUTES>" => "Minute",
                "<SECONDS>" => "Second",
                "<THOUSANDS>" => "Millisecond",
                _ => "<Hour/Minute/Second/Millisecond>"
            };
        }

        public static class DiagnosticDescriptors
        {
            public static readonly DiagnosticDescriptor Rule0083BuiltInDateTimeMethod = new(
                id: LinterCopAnalyzers.AnalyzerPrefix + "0083",
                title: LinterCopAnalyzers.GetLocalizableString("Rule0083BuiltInDateTimeMethodTitle"),
                messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0083BuiltInDateTimeMethodFormat"),
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
                description: LinterCopAnalyzers.GetLocalizableString("Rule0083BuiltInDateTimeMethodDescription"),
                helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0082");
        }
    }
}
#endif