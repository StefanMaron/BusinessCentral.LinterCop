using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0084UseReturnValueForErrorHandling : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0084UseReturnValueForErrorHandling);

    private static readonly HashSet<string> methodsToCheck = ["Find", "FindFirst", "FindLast", "Get", "GetBySystemId"];

    public override void Initialize(AnalysisContext context) =>
        context.RegisterOperationAction(AnalyzeAssignmentStatement, OperationKind.InvocationExpression);

    private void AnalyzeAssignmentStatement(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.TargetMethod.ContainingSymbol?.Name != "Table" ||
            !methodsToCheck.Contains(operation.TargetMethod.Name))
            return;

        if (ctx.Operation.Syntax.Parent.Kind == SyntaxKind.ExpressionStatement)
        {
            var methodName = operation.TargetMethod.Name.ToString();
            var node = operation.Syntax.DescendantNodesAndSelf()
                    .OfType<IdentifierNameSyntax>()
                    .FirstOrDefault(node => string.Equals(node.Identifier.ValueText, methodName, StringComparison.OrdinalIgnoreCase));

            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0084UseReturnValueForErrorHandling,
                node.GetLocation(),
                methodName));
        }
    }
}