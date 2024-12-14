using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0081UseIsEmptyMethod : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DiagnosticDescriptors.Rule0081UseIsEmptyMethod);

        public override void Initialize(AnalysisContext context) =>
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeCountMethod), OperationKind.InvocationExpression);

        private void AnalyzeCountMethod(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved())
                return;

            if (ctx.Operation is not IInvocationExpression operation)
                return;

            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
                operation.TargetMethod.Name != "Count" ||
                operation.TargetMethod.ContainingSymbol?.Name != "Table")
                return;

            if (operation.Instance?.GetSymbol() is not IVariableSymbol { Type: IRecordTypeSymbol recordTypeSymbol } || recordTypeSymbol.Temporary)
                return;

            if (operation.Syntax.Parent is not BinaryExpressionSyntax binaryExpression)
                return;

            if (IsLiteralExpressionValue(binaryExpression.Left, 0) ||
                IsLiteralExpressionValue(binaryExpression.Right, 0))
            {
                ReportDiagnostic(ctx, operation);
                return;
            }

            if (binaryExpression.OperatorToken.Kind == SyntaxKind.LessThanToken &&
                IsLiteralExpressionValue(binaryExpression.Right, 1))
            {
                ReportDiagnostic(ctx, operation);
                return;
            }

            if (binaryExpression.OperatorToken.Kind == SyntaxKind.GreaterThanToken &&
                IsLiteralExpressionValue(binaryExpression.Left, 1))
            {
                ReportDiagnostic(ctx, operation);
            }
        }

        private static bool IsLiteralExpressionValue(CodeExpressionSyntax codeExpression, int value) =>
            codeExpression is LiteralExpressionSyntax { Literal: { Kind: SyntaxKind.Int32SignedLiteralValue } literal }
            && literal.GetLiteralValue() is int literalvalue && literalvalue == value;

        private static void ReportDiagnostic(OperationAnalysisContext ctx, IInvocationExpression operation)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0081UseIsEmptyMethod,
                operation.Syntax.Parent.GetLocation(),
                new object[] { operation.Instance?.GetSymbol()?.Name.QuoteIdentifierIfNeeded() ?? string.Empty }));
        }

        public static class DiagnosticDescriptors
        {
            public static readonly DiagnosticDescriptor Rule0081UseIsEmptyMethod = new(
                id: LinterCopAnalyzers.AnalyzerPrefix + "0081",
                title: LinterCopAnalyzers.GetLocalizableString("Rule0081UseIsEmptyMethodTitle"),
                messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0081UseIsEmptyMethodFormat"),
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
                description: LinterCopAnalyzers.GetLocalizableString("Rule0081UseIsEmptyMethodDescription"),
                helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0081");
        }
    }
}
