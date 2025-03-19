using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

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

        bool skipDueToLocalOrGlobalVariable = optionDataType.Parent is SimpleTypeReferenceSyntax && !IsParameterOrReturnValue(optionDataType);
        if (skipDueToLocalOrGlobalVariable)
            return;

        bool skipDueToIsEventSubscriber = ctx.ContainingSymbol is IMethodSymbol method && method.IsEventSubscriber();
        if (skipDueToIsEventSubscriber)
            return;

        bool skipDueToTableIsOfTypeCDS = ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol() is ITableTypeSymbol table && table.TableType == TableTypeKind.CDS;
        if (skipDueToTableIsOfTypeCDS)
            return;

        // Handle FlowField with CalcFormula to a Field of Type option
        if (optionDataType.Parent is FieldSyntax fieldSyntax)
        {
            var calcFormulaPropertySyntax = fieldSyntax.PropertyList?.Properties
                .OfType<PropertySyntax>()
                .Select(p => p.Value)
                .OfType<CalculationFormulaPropertyValueSyntax>()
                .FirstOrDefault();

            if (calcFormulaPropertySyntax is not null &&
                calcFormulaPropertySyntax is FieldCalculationFormulaSyntax fieldCalculation &&
                fieldCalculation.Field is QualifiedNameSyntax qualifiedNameSyntax &&
                ctx.SemanticModel.GetSymbolInfo(qualifiedNameSyntax, ctx.CancellationToken).Symbol is ISymbol fieldSymbol &&
                fieldSymbol.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.Option)
            {
                return;
            }
        }

        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0088AvoidOptionTypes,
            optionDataType.GetLocation()
        ));
    }

    private static bool IsParameterOrReturnValue(OptionDataTypeSyntax optionDataType)
    {
        var parent = optionDataType.Parent?.Parent;
        return parent is not null && (parent.Kind == SyntaxKind.Parameter || parent.Kind == SyntaxKind.ReturnValue);
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
                HasVariablesNotInSource = ((IMethodSymbol)containingSymbol.OriginalDefinition).LocalVariables
                                                                .Where(var => !var.Type.GetLocation().IsInSource)
                                                                .Where(var => LocalVariablesName?.Contains(var.OriginalDefinition.Name) == true)
                                                                .Any();
                break;

            case SymbolKind.GlobalVariable:
                IApplicationObjectTypeSymbol? applicationObjectTypeSymbol = variable.GetContainingApplicationObjectTypeSymbol();
                if (applicationObjectTypeSymbol is null)
                    return;

                var GlobalVariablesName = GetReferencedVariableNames(applicationObjectTypeSymbol, variable);
                HasVariablesNotInSource = applicationObjectTypeSymbol.GetMembers()
                    .Where(member => member.Kind == SymbolKind.GlobalVariable || member.Kind == SymbolKind.Method)
                    .SelectMany(member => member is IMethodSymbol methodSymbol
                        ? methodSymbol.LocalVariables
                        : member is IVariableSymbol variableSymbol ? Enumerable.Repeat(variableSymbol, 1) : Enumerable.Empty<IVariableSymbol>())
                    .Where(var => !var.Type.GetLocation().IsInSource)
                    .Where(var => GlobalVariablesName?.Contains(var.OriginalDefinition.Name) == true)
                    .Any();
                break;
        }

        if (HasVariablesNotInSource == false)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0088AvoidOptionTypes,
                variable.GetLocation()
            ));
        }
    }

    private static IEnumerable<string>? GetReferencedVariableNames(ISymbol? containingSymbol, IVariableSymbol variable)
    {
        SyntaxNode? syntaxNode = containingSymbol?.DeclaringSyntaxReference?.GetSyntax();
        if (syntaxNode is null)
            return null;

        var nodes = syntaxNode.DescendantNodes()
            .OfType<ArgumentListSyntax>()
            .Where(argList => argList.Arguments.Any(argument =>
                (argument is OptionAccessExpressionSyntax optionAccess &&
                 optionAccess.Expression.GetIdentifierOrLiteralValue() == variable.Name) ||
                argument.GetIdentifierOrLiteralValue() == variable.Name));

        return nodes
            .SelectMany(node => node.AncestorsAndSelf()
                .OfType<ExpressionStatementSyntax>()
                .SelectMany(exprStmt => exprStmt.DescendantNodes()
                    .OfType<MemberAccessExpressionSyntax>()
                    .Select(memberAccess => memberAccess.Expression.ToString().UnquoteIdentifier())))
            .Distinct();
    }
}