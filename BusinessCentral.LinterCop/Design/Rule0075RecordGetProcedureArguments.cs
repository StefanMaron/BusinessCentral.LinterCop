using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0075RecordGetProcedureArguments : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0075RecordGetProcedureArguments);

    private static readonly Dictionary<NavTypeKind, HashSet<NavTypeKind>> ImplicitConversions = new()
    {
        // Integer can be converted to Option, BigInteger and/or Enum
        { NavTypeKind.Integer, new HashSet<NavTypeKind> { NavTypeKind.Option, NavTypeKind.BigInteger, NavTypeKind.Enum } },

        // BigInteger can be converted to Duration
        { NavTypeKind.BigInteger, new HashSet<NavTypeKind> { NavTypeKind.Duration } },

        // Code can be converted to Text
        { NavTypeKind.Code, new HashSet<NavTypeKind> { NavTypeKind.Text } },

        // Text can be converted to Code
        { NavTypeKind.Text, new HashSet<NavTypeKind> { NavTypeKind.Code } },

        // String(literal) can be converted to Text and/or Code
        { NavTypeKind.String, new HashSet<NavTypeKind> { NavTypeKind.Text, NavTypeKind.Code } }
    };

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterOperationAction(AnalyzeAssignmentStatement, OperationKind.InvocationExpression);
    }

    private void AnalyzeAssignmentStatement(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            !SemanticFacts.IsSameName(operation.TargetMethod.Name, "Get"))
            return;

        // Skip unsupported single argument scenarios, like Record.Get(RecordId)
        if (operation.Arguments.Length == 1 &&
            operation.Arguments[0].Value is IConversionExpression { Operand.Type: { } type } &&
            type.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.RecordId)
        {
            return;
        }

        if (operation.Instance?.Type.GetTypeSymbol().OriginalDefinition is not ITableTypeSymbol table)
            return;

        if (IsSingletonTable(table))
        {
            if (operation.Arguments.Length == 0)
                return;

            if (operation.Arguments.Length == 1 &&
                 operation.Arguments[0].Value is IConversionExpression { Operand.ConstantValue: { HasValue: true } constant } &&
                constant.Value?.ToString() == "")
            {
                return;
            }
        }

        if (operation.Arguments.Length != table.PrimaryKey.Fields.Length)
        {
            string expectedArgs = operation.Arguments.Length < table.PrimaryKey.Fields.Length
                ? $"Insufficient arguments provided; expected {table.PrimaryKey.Fields.Length} arguments"
                : $"Too many arguments provided; expected {table.PrimaryKey.Fields.Length} arguments";

            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0075RecordGetProcedureArguments,
                ctx.Operation.Syntax.GetLocation(), new object[] { table.Name.QuoteIdentifierIfNeeded(), expectedArgs }));
            return;
        }

        for (int i = 0; i < operation.Arguments.Length; i++)
        {
            if (!AreFieldCompatible(operation.Arguments[i], table.PrimaryKey.Fields[i]))
            {
                var argumentType = GetTypeSymbol(operation.Arguments[i]);
                var fieldType = table.PrimaryKey.Fields[i].Type;

                string expectedArgs = $"Argument at position {i + 1} has an invalid type; expected '{fieldType}', found '{argumentType}'";

                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0075RecordGetProcedureArguments,
                    ctx.Operation.Syntax.GetLocation(), new object[] { table.Name.QuoteIdentifierIfNeeded(), expectedArgs }));
                return;
            }
        }
    }

    private bool AreFieldCompatible(IArgument argument, IFieldSymbol field)
    {
        var argumentType = GetTypeSymbol(argument);
        var fieldType = field.Type;

        if (argumentType == null || fieldType is null)
            return true;

        var argumentNavType = argumentType.GetNavTypeKindSafe();
        var fieldNavType = fieldType.GetNavTypeKindSafe();

        if (argumentNavType == NavTypeKind.Enum && fieldNavType == NavTypeKind.Enum)
            return argumentType.OriginalDefinition == fieldType.OriginalDefinition;

        if (argumentNavType == fieldNavType ||
            argumentNavType == NavTypeKind.None ||
            argumentNavType == NavTypeKind.Joker)
            return true;

        if (ImplicitConversions.TryGetValue(argumentNavType, out var compatibleTypes) && !compatibleTypes.Contains(fieldNavType))
            return false;

        return true;
    }

    private static ITypeSymbol? GetTypeSymbol(IArgument argument)
    {
        switch (argument.Value.Kind)
        {
            case OperationKind.ConversionExpression:
                return ((IConversionExpression)argument.Value).Operand.Type;
            case OperationKind.InvocationExpression:
                return ((IInvocationExpression)argument.Value).TargetMethod.ReturnValueSymbol.ReturnType;
        }
        return null;
    }

    private static bool IsSingletonTable(ITableTypeSymbol table)
    {
        return table.PrimaryKey.Fields.Length == 1 &&
            table.PrimaryKey.Fields[0].OriginalDefinition.GetTypeSymbol() is { } typeSymbol &&
            typeSymbol.GetNavTypeKindSafe() == NavTypeKind.Code;
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0075RecordGetProcedureArguments = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0075",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0075RecordGetProcedureArgumentsTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0075RecordGetProcedureArgumentsFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0075RecordGetProcedureArgumentsDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0075");
    }
}