using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

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
    }
}
