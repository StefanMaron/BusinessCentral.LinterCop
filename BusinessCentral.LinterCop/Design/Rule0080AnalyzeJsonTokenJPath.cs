using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0080AnalyzeJsonTokenJPath : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0080AnalyzeJsonTokenJPath);

        public override void Initialize(AnalysisContext context) =>
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeSelectToken), OperationKind.InvocationExpression);

        private void AnalyzeSelectToken(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved())
                return;

            if (ctx.Operation is not IInvocationExpression operation)
                return;

            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
                operation.TargetMethod.Name != "SelectToken" ||
                operation.TargetMethod.ContainingSymbol?.Name != "JsonToken")
                return;

            var stringLiterals = operation.Arguments
                .Where(p => p.Parameter?.Name == "Path")
                .Select(p => p.Syntax)
                .OfType<LiteralExpressionSyntax>()
                .Select(l => l.Literal)
                .OfType<StringLiteralValueSyntax>()
                .Where(l => l.Value.ValueText?.Contains('"') ?? false);

            foreach (var stringLiteral in stringLiterals)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0080AnalyzeJsonTokenJPath,
                    stringLiteral.GetLocation()));
            }
        }

        public static class DiagnosticDescriptors
        {
            public static readonly DiagnosticDescriptor Rule0080AnalyzeJsonTokenJPath = new(
                id: LinterCopAnalyzers.AnalyzerPrefix + "0080",
                title: LinterCopAnalyzers.GetLocalizableString("Rule0080AnalyzeJsonTokenJPathTitle"),
                messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0080AnalyzePathOnJsonTokenFormat"),
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
                description: LinterCopAnalyzers.GetLocalizableString("Rule0080AnalyzePathOnJsonTokenDescription"),
                helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0080");
        }
    }
}
