using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Semantics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0017WriteToFlowField : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0017WriteToFlowField, DiagnosticDescriptors.Rule0000ErrorInRule);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeAssignmentStatement), OperationKind.AssignmentStatement);
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocationExpression), OperationKind.InvocationExpression);
    }

    private void AnalyzeInvocationExpression(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.TargetMethod.Name != "Validate" ||
            operation.TargetMethod.ContainingSymbol?.Name != "Table")
            return;

        if (operation.Arguments.Length < 1 ||
            operation.Arguments[0].Value is not IConversionExpression conversionExpression ||
            conversionExpression.Operand is not IFieldAccess fieldAccess ||
            fieldAccess.FieldSymbol.FieldClass != FieldClassKind.FlowField ||
            HasExplainingComment(operation))
            return;

        ctx.ReportDiagnostic(
            Diagnostic.Create(DiagnosticDescriptors.Rule0017WriteToFlowField,
            operation.Arguments[0].Value.Syntax.GetLocation()));
    }

    private void AnalyzeAssignmentStatement(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Operation is not IAssignmentStatement operation)
            return;

        if (operation.Target.Kind != OperationKind.FieldAccess)
            return;

        var fieldSymbol = ExtractFieldSymbolFromAssignment(operation);
        if (fieldSymbol?.FieldClass != FieldClassKind.FlowField || HasExplainingComment(operation))
            return;

        ctx.ReportDiagnostic(
            Diagnostic.Create(DiagnosticDescriptors.Rule0017WriteToFlowField,
            operation.Target.Syntax.GetLocation()));
    }

    private IFieldSymbol? ExtractFieldSymbolFromAssignment(IAssignmentStatement operation)
    {
        if (operation.Target.Syntax.Kind == SyntaxKind.ArrayIndexExpression &&
            operation.Target is ITextIndexAccess textIndexAccess &&
            textIndexAccess.TextExpression is IFieldAccess fieldAccess)
        {
            return fieldAccess.FieldSymbol;
        }

        return operation.Target is IFieldAccess directFieldAccess ? directFieldAccess.FieldSymbol : null;
    }

    private bool HasExplainingComment(IOperation operation) =>
        operation.Syntax.GetLeadingTrivia().Any(trivia => trivia.IsKind(SyntaxKind.LineCommentTrivia));
}