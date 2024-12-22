#if !LessThenFall2023RV1
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0043SecretText : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0043SecretText);

    public override VersionCompatibility SupportedVersions => VersionCompatibility.Fall2023OrGreater;

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
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeIsolatedStorage), OperationKind.InvocationExpression);
    }

    private void AnalyzeIsolatedStorage(OperationAnalysisContext ctx)
    {
#if !LessThenSpring2024
        if (!VersionChecker.IsSupported(ctx.ContainingSymbol, VersionCompatibility.Spring2024OrGreater))
            return;

        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod is not IMethodSymbol targetMethod)
            return;

        if (targetMethod.ContainingSymbol?.Kind != SymbolKind.Class)
            return;

        if (!SemanticFacts.IsSameName(targetMethod.ContainingSymbol.Name, "IsolatedStorage"))
            return;

        IArgument? argument = operation.TargetMethod.Name.ToLowerInvariant() switch
        {
            "get" => operation.Arguments.FirstOrDefault(a => a.Parameter.IsVar),
            "set" or "setencrypted" when operation.Arguments.Length > 1 => operation.Arguments[1],
            _ => null
        };

        if (argument is null)
            return;

        if (!IsArgumentOfTypeSecretText(argument))
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0043SecretText,
                argument.Syntax.GetLocation()));
#endif
    }

    private void AnalyzeHttpObjects(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        // We need at least two arguments
        if (operation.Arguments.Length < 2) return;

        switch (operation.TargetMethod.MethodKind)
        {
            case MethodKind.BuiltInMethod:
                if (!buildInMethodNames.Contains(operation.TargetMethod.Name.ToLowerInvariant()))
                    return;

                if (!(operation.Instance?.GetSymbol()?.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.HttpHeaders || operation.Instance?.GetSymbol()?.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.HttpClient))
                    return;

                break;
            case MethodKind.Method:
                if (operation.TargetMethod.ContainingType?.GetNavTypeKindSafe() != NavTypeKind.Codeunit)
                    return;
                ICodeunitTypeSymbol codeunitTypeSymbol = (ICodeunitTypeSymbol)operation.TargetMethod.GetContainingObjectTypeSymbol();
                if (!SemanticFacts.IsSameName(((INamespaceSymbol)codeunitTypeSymbol.ContainingSymbol!).QualifiedName, "System.RestClient"))
                    return;
                if (!SemanticFacts.IsSameName(codeunitTypeSymbol.Name, "Rest Client"))
                    return;
                if (!SemanticFacts.IsSameName(operation.TargetMethod.Name, "SetDefaultRequestHeader"))
                    return;
                break;
            default:
                return;
        }

        if (!IsAuthorizationArgument(operation.Arguments[0]))
            return;

        if (!IsArgumentOfTypeSecretText(operation.Arguments[1]))
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0043SecretText,
                ctx.Operation.Syntax.GetLocation()));
    }

    private bool IsArgumentOfTypeSecretText(IArgument argument)
    {
        return argument.Parameter?.OriginalDefinition.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.SecretText;
    }

    private static bool IsAuthorizationArgument(IArgument argument)
    {
        switch (argument.Syntax.Kind)
        {
            case SyntaxKind.LiteralExpression:
                return SemanticFacts.IsSameName(argument.Value.ConstantValue.Value.ToString(), authorization);
            case SyntaxKind.IdentifierName:
                if (argument.Value.Kind != OperationKind.ConversionExpression) return false;
                IOperation operand = ((IConversionExpression)argument.Value).Operand;
                if (operand.GetSymbol()?.OriginalDefinition.GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Label) return false;
                ILabelTypeSymbol label = (ILabelTypeSymbol)operand.GetSymbol()!.OriginalDefinition.GetTypeSymbol();
                return SemanticFacts.IsSameName(label.GetLabelText(), authorization);
            default:
                return false;
        }
    }
}
#endif