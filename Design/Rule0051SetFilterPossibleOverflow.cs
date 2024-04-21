using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0051SetFilterPossibleOverflow : DiagnosticAnalyzer
    {
        private readonly Lazy<Regex> strSubstNoPatternLazy = new Lazy<Regex>((Func<Regex>)(() => new Regex("[#%](\\d+)", RegexOptions.Compiled)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0051SetFilterPossibleOverflow);

        private Regex StrSubstNoPattern => this.strSubstNoPatternLazy.Value;

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeInvocation), OperationKind.InvocationExpression);

        private void AnalyzeInvocation(OperationAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            if (operation.TargetMethod == null || !SemanticFacts.IsSameName(operation.TargetMethod.Name, "SetFilter") || operation.Arguments.Count() < 3)
                return;

            if (operation.Arguments[0].Value.Kind != OperationKind.ConversionExpression) return;
            IOperation fieldOperand = ((IConversionExpression)operation.Arguments[0].Value).Operand;
            ITypeSymbol fieldType = fieldOperand.Type;
            if (fieldType.GetNavTypeKindSafe() == NavTypeKind.Text) return;

            bool isError = false;
            int typeLength = GetTypeLength(fieldType, ref isError);
            if (isError || typeLength == int.MaxValue)
                return;

            foreach (int argIndex in GetArgumentIndexes(operation.Arguments[1].Value))
            {
                int index = argIndex + 1; // The placeholders are defines as %1, %2, %3, where in case of %1 we need the second (zero based) index of the arguments of the SetFilter method
                if (index < 2 || index >= operation.Arguments.Count()) continue;

                int expressionLength = this.CalculateMaxExpressionLength(((IConversionExpression)operation.Arguments[index].Value).Operand, ref isError);
                if (!isError && expressionLength > typeLength)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0051SetFilterPossibleOverflow, operation.Syntax.GetLocation(), GetDisplayString(operation.Arguments[index], operation), GetDisplayString(operation.Arguments[0], operation)));
            }
        }

        private static int GetTypeLength(ITypeSymbol type, ref bool isError)
        {
            if (!type.IsTextType())
            {
                isError = true;
                return 0;
            }
            if (type.HasLength)
                return type.Length;
            return type.NavTypeKind == NavTypeKind.Label ? GetLabelTypeLength(type) : int.MaxValue;
        }

        private static int GetLabelTypeLength(ITypeSymbol type)
        {
            ILabelTypeSymbol labelType = (ILabelTypeSymbol)type;

            if (labelType.Locked)
                return labelType.GetLabelText().Length;

            return labelType.MaxLength;
        }

        private int CalculateMaxExpressionLength(IOperation expression, ref bool isError)
        {
            if (expression.Syntax.Parent.IsKind(SyntaxKind.CaseLine))
            {
                isError = true;
                return 0;
            }
            switch (expression.Kind)
            {
                case OperationKind.LiteralExpression:
                    if (expression.Type.IsTextType())
                        return expression.ConstantValue.Value.ToString().Length;
                    ITypeSymbol type = expression.Type;
                    if ((type != null ? (type.NavTypeKind == NavTypeKind.Char ? 1 : 0) : 0) != 0)
                        return 1;
                    break;
                case OperationKind.ConversionExpression:
                    return this.CalculateMaxExpressionLength(((IConversionExpression)expression).Operand, ref isError);
                case OperationKind.InvocationExpression:
                    IInvocationExpression invocation = (IInvocationExpression)expression;
                    IMethodSymbol targetMethod = invocation.TargetMethod;
                    if (targetMethod != null && targetMethod.ContainingSymbol?.Kind == SymbolKind.Class)
                    {
                        switch (targetMethod.Name.ToLowerInvariant())
                        {
                            case "convertstr":
                            case "delchr":
                            case "delstr":
                            case "incstr":
                            case "lowercase":
                            case "uppercase":
                                if (invocation.Arguments.Length > 0)
                                    return this.CalculateBuiltInMethodResultLength(invocation, 0, ref isError);
                                break;
                            case "copystr":
                                if (invocation.Arguments.Length == 3)
                                    return this.CalculateBuiltInMethodResultLength(invocation, 2, ref isError);
                                break;
                            case "format":
                                return 0;
                            case "padstr":
                            case "substring":
                                if (invocation.Arguments.Length >= 2)
                                    return this.CalculateBuiltInMethodResultLength(invocation, 1, ref isError);
                                break;
                            case "strsubstno":
                                if (invocation.Arguments.Length > 0)
                                    return this.CalculateStrSubstNoMethodResultLength(invocation, ref isError);
                                break;
                            case "tolower":
                            case "toupper":
                                if (invocation.Instance.IsBoundExpression())
                                    return GetTypeLength(invocation.Instance.Type, ref isError);
                                break;
                        }
                    }
                    return GetTypeLength(expression.Type, ref isError);
                case OperationKind.LocalReferenceExpression:
                case OperationKind.GlobalReferenceExpression:
                case OperationKind.ReturnValueReferenceExpression:
                case OperationKind.ParameterReferenceExpression:
                case OperationKind.FieldAccess:
                    return GetTypeLength(expression.Type, ref isError);
                case OperationKind.BinaryOperatorExpression:
                    IBinaryOperatorExpression operatorExpression = (IBinaryOperatorExpression)expression;
                    return Math.Min(int.MaxValue, this.CalculateMaxExpressionLength(operatorExpression.LeftOperand, ref isError) + this.CalculateMaxExpressionLength(operatorExpression.RightOperand, ref isError));
            }
            isError = true;
            return 0;
        }

        private static int? TryGetLength(IInvocationExpression invocation, int lengthArgPos)
        {
            if (!(SemanticFacts.GetBoundExpressionArgument(invocation, lengthArgPos) is IConversionExpression expressionArgument))
                return new int?();
            ITypeSymbol type = expressionArgument.Operand.Type;
            return type.HasLength ? new int?(type.Length) : new int?();
        }

        private int CalculateBuiltInMethodResultLength(
          IInvocationExpression invocation,
          int lengthArgPos,
          ref bool isError)
        {
            IOperation operation = invocation.Arguments[lengthArgPos].Value;
            switch (operation.Kind)
            {
                case OperationKind.LiteralExpression:
                    Optional<object> constantValue = operation.ConstantValue;
                    if (constantValue.HasValue)
                    {
                        if (operation.Type.IsIntegralType())
                        {
                            constantValue = operation.ConstantValue;
                            return (int)constantValue.Value;
                        }
                        if (operation.Type.IsTextType())
                        {
                            constantValue = operation.ConstantValue;
                            return constantValue.Value.ToString().Length;
                        }
                        break;
                    }
                    break;
                case OperationKind.InvocationExpression:
                    invocation = (IInvocationExpression)operation;
                    IMethodSymbol targetMethod = invocation.TargetMethod;
                    if (targetMethod != null && SemanticFacts.IsSameName(targetMethod.Name, "maxstrlen") && targetMethod.ContainingSymbol?.Kind == SymbolKind.Class)
                    {
                        ImmutableArray<IArgument> arguments = invocation.Arguments;
                        if (arguments.Length == 1)
                        {
                            arguments = invocation.Arguments;
                            IOperation operand = arguments[0].Value;
                            if (operand.Kind == OperationKind.ConversionExpression)
                                operand = ((IConversionExpression)operand).Operand;
                            return GetTypeLength(operand.Type, ref isError);
                        }
                        break;
                    }
                    break;
            }
            return TryGetLength(invocation, lengthArgPos) ?? GetTypeLength(invocation.Type, ref isError);
        }

        private int CalculateStrSubstNoMethodResultLength(
          IInvocationExpression invocation,
          ref bool isError)
        {
            IOperation operation = invocation.Arguments[0].Value;
            if (!operation.Type.IsTextType())
            {
                isError = true;
                return -1;
            }
            Optional<object> constantValue = operation.ConstantValue;
            if (!constantValue.HasValue)
            {
                isError = true;
                return -1;
            }
            constantValue = operation.ConstantValue;
            string input = constantValue.Value.ToString();
            Match match = this.StrSubstNoPattern.Match(input);
            int num;
            for (num = input.Length; !isError && match.Success && num < int.MaxValue; match = match.NextMatch())
            {
                string s = match.Groups[1].Value;
                int result = 0;
                if (int.TryParse(s, out result) && 0 < result && result < invocation.Arguments.Length)
                {
                    int expressionLength = this.CalculateMaxExpressionLength(invocation.Arguments[result].Value, ref isError);
                    num = expressionLength == int.MaxValue ? expressionLength : num + expressionLength - s.Length - 1;
                }
            }
            return !isError ? num : -1;
        }

        private static string GetDisplayString(IArgument argument, IInvocationExpression operation)
        {
            return ((IConversionExpression)argument.Value).Operand.Type.ToDisplayString();
        }

        private List<int> GetArgumentIndexes(IOperation operand)
        {
            List<int> results = new List<int>();

            if (operand.Syntax.Kind != SyntaxKind.LiteralExpression)
                return results;

            foreach (Match match in this.StrSubstNoPattern.Matches(operand.Syntax.ToFullString()))
            {
                if (int.TryParse(match.Groups[1].Value, out int number))
                    if (!results.Contains(number))
                        results.Add(number);
            }

            return results;
        }
    }
}