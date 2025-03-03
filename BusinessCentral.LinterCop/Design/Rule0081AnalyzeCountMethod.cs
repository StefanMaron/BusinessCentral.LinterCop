using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0081AnalyzeCountMethod : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DiagnosticDescriptors.Rule0081UseIsEmptyMethod,
            DiagnosticDescriptors.Rule0082UseQueryOrFindWithNext);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeCountMethod), OperationKind.InvocationExpression);

    private void AnalyzeCountMethod(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
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

        if (IsEligibleUseQueryOrFindWithNext(recordTypeSymbol))
        {
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

    // Tables with one of these identifiers in the name could possible have a large amount of records
    private static readonly HashSet<string> possibleLargeTableIdentifierKeywords = new HashSet<string>
    {
        "Ledger", "GL", "G/L",
        "Posted", "Pstd",
        "Log",
        "Entry",
        "Archive",
    };

    private bool IsEligibleUseQueryOrFindWithNext(IRecordTypeSymbol record)
    {
        if (possibleLargeTableIdentifierKeywords.Any(keyword => record.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
            return true;

        // Tables with a field "Entry No." could possible have a large amount of records
        if (record.OriginalDefinition is ITableTypeSymbol table)
            return table.PrimaryKey.Fields.Any(field => string.Equals(field.Name, "Entry No.", StringComparison.OrdinalIgnoreCase));

        return false;
    }

    private static void ReportUseIsEmptyDiagnostic(OperationAnalysisContext ctx, IInvocationExpression operation)
    {
        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0081UseIsEmptyMethod,
            operation.Syntax.Parent.GetLocation(),
            GetSymbolName(operation)));
    }

    private static void ReportUseFindWithNextDiagnostic(OperationAnalysisContext ctx, IInvocationExpression operation, SyntaxKind operatorToken)
    {
        string operatorSign = operatorToken == SyntaxKind.EqualsToken ? "=" : "<>";

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0082UseQueryOrFindWithNext,
            operation.Syntax.Parent.GetLocation(),
            GetSymbolName(operation), operatorSign));
    }

    private static string GetSymbolName(IInvocationExpression operation) =>
            operation.Instance?.GetSymbol()?.Name.QuoteIdentifierIfNeeded() ?? string.Empty;
}