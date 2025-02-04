using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0088AvoidOptionTypes : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0088AvoidOptionTypes);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(
            new Action<SyntaxNodeAnalysisContext>(this.AnalyzeSyntaxNodes),
            new SyntaxKind[]{
                    SyntaxKind.OptionDataType
            }
        );

        context.RegisterSymbolAction(
            new Action<SymbolAnalysisContext>(this.AnalyzeVariables),
            SymbolKind.GlobalVariable,
            SymbolKind.LocalVariable);
    }

    private void AnalyzeSyntaxNodes(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Node is not OptionDataTypeSyntax optionDataType)
        {
            return;
        }

        if (optionDataType.Parent is SimpleTypeReferenceSyntax ||
            ctx.ContainingSymbol is IMethodSymbol method && method.IsEventSubscriber() ||
            ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol() is ITableTypeSymbol table && table.TableType == TableTypeKind.CDS)
        {
            return;
        }

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0088AvoidOptionTypes,
            optionDataType.GetLocation(),
            new object[] { optionDataType.ToString() }
        ));
    }

    private void AnalyzeVariables(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IVariableSymbol variable)
            return;

        if (variable.Type.GetNavTypeKindSafe() != NavTypeKind.Option)
            return;

        bool? HasVariablesNotInSource = null;
        switch (variable.Kind)
        {
            case SymbolKind.LocalVariable:
                ISymbol? containingSymbol = variable.ContainingSymbol;
                if (containingSymbol is null)
                    return;

                var LocalVariablesName = GetReferencedVariableNames(containingSymbol, variable);
                if (LocalVariablesName is null)
                    return;

                HasVariablesNotInSource = ((IMethodSymbol)containingSymbol.OriginalDefinition).LocalVariables
                                                                .Where(var => !var.Type.GetLocation().IsInSource)
                                                                .Where(var => LocalVariablesName.Contains(var.OriginalDefinition.Name))
                                                                .Any();
                break;

            case SymbolKind.GlobalVariable:
                IApplicationObjectTypeSymbol? applicationObjectTypeSymbol = variable.GetContainingApplicationObjectTypeSymbol();
                if (applicationObjectTypeSymbol is null)
                    return;

                var GlobalVariablesName = GetReferencedVariableNames(applicationObjectTypeSymbol, variable);
                if (GlobalVariablesName is null)
                    return;

                HasVariablesNotInSource = applicationObjectTypeSymbol.GetMembers()
                    .Where(member => member.Kind == SymbolKind.GlobalVariable || member.Kind == SymbolKind.Method)
                    .SelectMany(member => member is IMethodSymbol methodSymbol
                        ? methodSymbol.LocalVariables
                        : member is IVariableSymbol variableSymbol ? Enumerable.Repeat(variableSymbol, 1) : Enumerable.Empty<IVariableSymbol>())
                    .Where(var => !var.Type.GetLocation().IsInSource)
                    .Where(var => GlobalVariablesName.Contains(var.OriginalDefinition.Name))
                    .Any();
                break;
        }

        if (HasVariablesNotInSource == false)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0088AvoidOptionTypes,
                variable.GetLocation(),
                variable.Name
            ));
        }
    }

    private static List<string>? GetReferencedVariableNames(ISymbol? containingSymbol, IVariableSymbol variable)
    {
        var nodes = GetReferencedNodes(containingSymbol, variable.Name);
        return GetDistinctVariableNames(nodes);
    }

    private static IEnumerable<OptionAccessExpressionSyntax>? GetReferencedNodes(ISymbol? symbol, string variableName)
    {
        if (symbol is null)
            return null;

        SyntaxNode? syntaxNode = symbol.DeclaringSyntaxReference?.GetSyntax();
        if (syntaxNode is null)
            return null;

        return syntaxNode.DescendantNodes()
                .OfType<OptionAccessExpressionSyntax>()
                .Where(node => node.Expression.GetIdentifierOrLiteralValue() == variableName);
    }

    private static List<string>? GetDistinctVariableNames(IEnumerable<OptionAccessExpressionSyntax>? nodes)
    {
        if (nodes is null)
            return null;

        var variableNames = new List<string>();

        foreach (var node in nodes)
        {
            var parentNode = node.Parent;
            while (parentNode is not null && parentNode.Kind != SyntaxKind.ExpressionStatement)
            {
                parentNode = parentNode.Parent;
            }

            if (parentNode is ExpressionStatementSyntax expressionStatement &&
                expressionStatement.Expression is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var variableName = memberAccess.Expression.ToString();
                if (!variableNames.Contains(variableName))
                    variableNames.Add(variableName);
            }
        }

        if (variableNames.Count == 0)
            return null;

        return variableNames;
    }
}