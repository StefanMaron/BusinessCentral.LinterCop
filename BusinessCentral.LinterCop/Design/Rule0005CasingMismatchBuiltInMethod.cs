using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0005CasingMismatchBuiltInMethod : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create(DiagnosticDescriptors.Rule0005CasingMismatch);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeBuiltInMethod), new OperationKind[] {
                OperationKind.InvocationExpression,
                OperationKind.FieldAccess,
                OperationKind.GlobalReferenceExpression,
                OperationKind.LocalReferenceExpression,
                OperationKind.ParameterReferenceExpression,
                OperationKind.ReturnValueReferenceExpression,
                OperationKind.XmlPortDataItemAccess
            });
    }

    private void AnalyzeBuiltInMethod(OperationAnalysisContext ctx)
    {
        var operation = ctx.Operation;
        string targetName = operation.Kind switch
        {
            OperationKind.InvocationExpression when operation is IInvocationExpression invocation
                => invocation.TargetMethod.Name,
            OperationKind.FieldAccess when operation is IFieldAccess fieldAccess
                => fieldAccess.FieldSymbol.Name,
            OperationKind.GlobalReferenceExpression
                => ((IGlobalReferenceExpression)operation).GlobalVariable.Name,
            OperationKind.LocalReferenceExpression
                => ((ILocalReferenceExpression)operation).LocalVariable.Name,
            OperationKind.ParameterReferenceExpression
                => ((IParameterReferenceExpression)operation).Parameter.Name,
            OperationKind.ReturnValueReferenceExpression
                => ((IReturnValueReferenceExpression)operation).ReturnValue.Name,
            OperationKind.XmlPortDataItemAccess
                => ((IXmlPortNodeAccess)operation).XmlPortNodeSymbol.Name,
            _ => string.Empty
        };
        if (string.IsNullOrEmpty(targetName))
            return;

        ReadOnlySpan<char> targetSpan = targetName.AsSpan();
        SyntaxNode opSyntax = operation.Syntax;
        var opSyntaxUnquoted = opSyntax.ToString().UnquoteIdentifier();

        if (OnlyDiffersInCasing(opSyntaxUnquoted.AsSpan(), targetSpan))
        {
            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.Rule0005CasingMismatch,
                    opSyntax.GetLocation(),
                    targetName.QuoteIdentifierIfNeeded()));
            return;
        }

        foreach (var descendant in opSyntax.DescendantNodes())
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            var descendantUnquoted = descendant.ToString().UnquoteIdentifier();
            if (OnlyDiffersInCasing(descendantUnquoted.AsSpan(), targetSpan))
            {
                ctx.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Rule0005CasingMismatch,
                        opSyntax.GetLocation(),
                        targetName.QuoteIdentifierIfNeeded()));
                return;
            }
        }
    }

    private bool OnlyDiffersInCasing(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
    {
        return left.Equals(right, StringComparison.OrdinalIgnoreCase) &&
               !left.Equals(right, StringComparison.Ordinal);
    }
}