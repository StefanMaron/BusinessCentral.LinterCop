using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0048ErrorWithTextConstant : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0048ErrorWithTextConstant);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeErrorMethod), OperationKind.InvocationExpression);

    private void AnalyzeErrorMethod(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.TargetMethod.Name != "Error" ||
            operation.Arguments.Length == 0 ||
            operation.Arguments[0].Value.Type.GetNavTypeKindSafe() == NavTypeKind.ErrorInfo)
            return;

        switch (operation.Arguments[0].Syntax.Kind)
        {
            case SyntaxKind.IdentifierName:
            case SyntaxKind.MemberAccessExpression:
                if (operation.Arguments[0].Value.Kind != OperationKind.ConversionExpression)
                    break;

                IOperation operand = ((IConversionExpression)operation.Arguments[0].Value).Operand;
                if (operand.GetSymbol()?.OriginalDefinition.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.Label)
                    return;

                break;

            case SyntaxKind.LiteralExpression:
                if (operation.Arguments[0].Syntax.GetIdentifierOrLiteralValue() == "")
                    return;

                break;
        }

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0048ErrorWithTextConstant,
            ctx.Operation.Syntax.GetLocation()));
    }
}