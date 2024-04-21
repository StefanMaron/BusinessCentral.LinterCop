using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0043SecretText : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0043SecretText);

        private static readonly string authorization = "Authorization";

        private static readonly List<string> buildInMethodNames = new List<string>
        {
            "add",
            "getvalues",
            "tryaddwithoutvalidation"
        };

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeHttpObjects), OperationKind.InvocationExpression);
            // TODO: enable after Spring2024OrGreater release
            // context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeIsolatedStorage), OperationKind.InvocationExpression);
        }

        private void AnalyzeIsolatedStorage(OperationAnalysisContext ctx)
        {
            // TODO: enable after Spring2024OrGreater release
            // if (!VersionChecker.IsSupported(ctx.ContainingSymbol, VersionCompatibility.Spring2024OrGreater)) return;

            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.Arguments.Count() < 3) return;

            IMethodSymbol targetMethod = operation.TargetMethod;
            if (targetMethod == null || targetMethod.ContainingSymbol.Kind != SymbolKind.Class) return;
            if (!SemanticFacts.IsSameName(targetMethod.ContainingSymbol.Name, "IsolatedStorage")) return;

            int argumentIndex;
            switch (operation.TargetMethod.Name.ToLowerInvariant())
            {
                case "get":
                    argumentIndex = 2;
                    break;
                case "set":
                case "setencrypted":
                    argumentIndex = 1;
                    break;
                default:
                    argumentIndex = -1;
                    break;
            }

            if (argumentIndex == -1 || operation.Arguments[argumentIndex].Parameter == null) return;

            if (!IsArgumentOfTypeSecretText(operation.Arguments[argumentIndex]))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0043SecretText, ctx.Operation.Syntax.GetLocation()));
        }

        private void AnalyzeHttpObjects(OperationAnalysisContext ctx)
        {
            if (!VersionChecker.IsSupported(ctx.ContainingSymbol, VersionCompatibility.Fall2023OrGreater)) return;

            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;

            // We need at least two arguments
            if (operation.Arguments.Count() < 2) return;

            switch (operation.TargetMethod.MethodKind)
            {
                case MethodKind.BuiltInMethod:
                    if (!buildInMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant())) return;
                    if (!(operation.Instance?.GetSymbol().GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.HttpHeaders || operation.Instance?.GetSymbol().GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.HttpClient)) return;
                    break;
                case MethodKind.Method:
                    if (operation.TargetMethod.ContainingType.GetNavTypeKindSafe() != NavTypeKind.Codeunit) return;
                    ICodeunitTypeSymbol codeunitTypeSymbol = (ICodeunitTypeSymbol)operation.TargetMethod.GetContainingObjectTypeSymbol();
#if Fall2023
                    if (!SemanticFacts.IsSameName(((INamespaceSymbol)codeunitTypeSymbol.ContainingSymbol).QualifiedName, "System.RestClient")) return;
#endif
                    if (!SemanticFacts.IsSameName(codeunitTypeSymbol.Name, "Rest Client")) return;
                    if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "SetDefaultRequestHeader")) return;
                    break;
                default:
                    return;
            }

            if (!IsAuthorizationArgument(operation.Arguments[0])) return;

            if (!IsArgumentOfTypeSecretText(operation.Arguments[1]))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0043SecretText, ctx.Operation.Syntax.GetLocation()));
        }

        private bool IsArgumentOfTypeSecretText(IArgument argument)
        {
            return argument.Parameter.OriginalDefinition.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.SecretText;
        }

        private static bool IsAuthorizationArgument(IArgument argument)
        {
            switch (argument.Syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return SemanticFacts.IsSameName(argument.Value.ConstantValue.Value.ToString(), authorization);
                case SyntaxKind.IdentifierName:
                    IOperation operand = ((IConversionExpression)argument.Value).Operand;
                    if (operand.GetSymbol().OriginalDefinition.GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Label) return false;
                    ILabelTypeSymbol label = (ILabelTypeSymbol)operand.GetSymbol().OriginalDefinition.GetTypeSymbol();
                    return SemanticFacts.IsSameName(label.GetLabelText(), authorization);
                default:
                    return false;
            }
        }
    }
}