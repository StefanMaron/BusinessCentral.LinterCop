#nullable disable // TODO: Enable nullable and review rule
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0042AutoCalcFieldsOnNormalFields : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0042AutoCalcFieldsOnNormalFields);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(syntaxContext =>
        {
            if (!syntaxContext.Node.ToString().ToLowerInvariant().Contains("setautocalcfields"))
                return;

            IInvocationExpression operation = (IInvocationExpression)syntaxContext.SemanticModel.GetOperation(syntaxContext.Node);
            IMethodSymbol targetMethod = operation.TargetMethod;
            if (targetMethod == null || !SemanticFacts.IsSameName(targetMethod.Name, "setautocalcfields") || targetMethod.MethodKind != MethodKind.BuiltInMethod)
                return;

            foreach (IArgument obj in operation.Arguments)
            {
                if ((obj.Value is IConversionExpression conversionExpression2 ? conversionExpression2.Operand : (IOperation)null) is IFieldAccess fieldAccess2 && fieldAccess2.FieldSymbol.FieldClass != FieldClassKind.FlowField && fieldAccess2.Type.NavTypeKind != NavTypeKind.Blob)
                    syntaxContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0042AutoCalcFieldsOnNormalFields, fieldAccess2.Syntax.GetLocation(), (object)fieldAccess2.FieldSymbol.Name));
            }
        }, SyntaxKind.InvocationExpression);
    }
}