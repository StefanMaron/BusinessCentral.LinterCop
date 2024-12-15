using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0081AnalyzeCountMethod : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DiagnosticDescriptors.Rule0081UseIsEmptyMethod, DiagnosticDescriptors.Rule0082UseFindWithNext);

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

            int rightValue = GetLiteralExpressionValue(binaryExpression.Right);
            if (rightValue > Literals.MaxRelevantValue)
                return;

            int leftValue = GetLiteralExpressionValue(binaryExpression.Left);
            if (leftValue > Literals.MaxRelevantValue)
                return;

            if (IsZeroComparison(leftValue, rightValue))
            {
                ReportUseIsEmptyDiagnostic(ctx, operation);
                return;
            }

            if (IsLessThanOneComparison(binaryExpression, rightValue) || IsGreaterThanOneComparison(binaryExpression, leftValue))
            {
                ReportUseIsEmptyDiagnostic(ctx, operation);
                return;
            }

            if (IsOneComparison(leftValue, rightValue))
            {
                ReportUseFindWithNextDiagnostic(ctx, operation, GetOperatorKind(binaryExpression.OperatorToken.Kind));
                return;
            }

            if (IsLessThanTwoComparison(binaryExpression, rightValue) || IsGreaterThanTwoComparison(binaryExpression, leftValue))
            {
                ReportUseFindWithNextDiagnostic(ctx, operation, SyntaxKind.EqualsToken);
                return;
            }
        }

        private static int GetLiteralExpressionValue(CodeExpressionSyntax codeExpression) =>
            codeExpression is LiteralExpressionSyntax { Literal.Kind: SyntaxKind.Int32SignedLiteralValue } literalExpression &&
            literalExpression.Literal.GetLiteralValue() is int value ? value : -1;

        private static SyntaxKind GetOperatorKind(SyntaxKind tokenKind) =>
            tokenKind == SyntaxKind.EqualsToken ? SyntaxKind.EqualsToken : SyntaxKind.NotEqualsToken;

        private static bool IsZeroComparison(int left, int right)
            => left == Literals.Zero || right == Literals.Zero;

        private static bool IsLessThanOneComparison(BinaryExpressionSyntax expr, int right) =>
                 expr.OperatorToken.Kind == SyntaxKind.LessThanToken && right == Literals.One;

        private static bool IsGreaterThanOneComparison(BinaryExpressionSyntax expr, int left) =>
            expr.OperatorToken.Kind == SyntaxKind.GreaterThanToken && left == Literals.One;

        private static bool IsOneComparison(int left, int right) =>
            left == Literals.One || right == Literals.One;

        private static bool IsLessThanTwoComparison(BinaryExpressionSyntax expr, int right) =>
            expr.OperatorToken.Kind == SyntaxKind.LessThanToken && right == Literals.Two;

        private static bool IsGreaterThanTwoComparison(BinaryExpressionSyntax expr, int left) =>
            expr.OperatorToken.Kind == SyntaxKind.GreaterThanToken && left == Literals.Two;

        private static class Literals
        {
            public const int Zero = 0;
            public const int One = 1;
            public const int Two = 2;
            public const int MaxRelevantValue = 2;
        }

        private static void ReportUseIsEmptyDiagnostic(OperationAnalysisContext ctx, IInvocationExpression operation)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0081UseIsEmptyMethod,
                operation.Syntax.Parent.GetLocation(),
                new object[] { GetSymbolName(operation) }));
        }

        private static void ReportUseFindWithNextDiagnostic(OperationAnalysisContext ctx, IInvocationExpression operation, SyntaxKind operatorToken)
        {
            string operatorSign = operatorToken == SyntaxKind.EqualsToken ? "=" : "<>";

            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0082UseFindWithNext,
                operation.Syntax.Parent.GetLocation(),
                new object[] { GetSymbolName(operation), operatorSign }));
        }

        private static string GetSymbolName(IInvocationExpression operation) =>
                operation.Instance?.GetSymbol()?.Name.QuoteIdentifierIfNeeded() ?? string.Empty;

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

            public static readonly DiagnosticDescriptor Rule0082UseFindWithNext = new(
                id: LinterCopAnalyzers.AnalyzerPrefix + "0082",
                title: LinterCopAnalyzers.GetLocalizableString("Rule0082UseFindWithNextTitle"),
                messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0082UseFindWithNextFormat"),
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
                description: LinterCopAnalyzers.GetLocalizableString("Rule0082UseFindWithNextDescription"),
                helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0082");
        }
    }
}