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
            SyntaxKind.OptionDataType);

        context.RegisterSymbolAction(
            new Action<SymbolAnalysisContext>(this.AnalyzeVariables),
            SymbolKind.GlobalVariable,
            SymbolKind.LocalVariable);
    }

    #region SyntaxNodeAnalysis 
    private void AnalyzeSyntaxNodes(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Node is not OptionDataTypeSyntax optionDataType)
            return;

        bool skipDueToLocalOrGlobalVariable = optionDataType.Parent is SimpleTypeReferenceSyntax && !IsParameterOrReturnValue(optionDataType);
        if (skipDueToLocalOrGlobalVariable)
            return;

        bool skipDueToIsEventSubscriber = ctx.ContainingSymbol is IMethodSymbol method && method.IsEventSubscriber();
        if (skipDueToIsEventSubscriber)
            return;

        bool skipDueToTableIsOfTypeCDS = ctx.ContainingSymbol.GetContainingApplicationObjectTypeSymbol() is ITableTypeSymbol table && table.TableType == TableTypeKind.CDS;
        if (skipDueToTableIsOfTypeCDS)
            return;

        if (optionDataType.Parent is FieldSyntax fieldSyntax &&
            IsFlowFieldWithOptionCalculation(fieldSyntax, ctx))
        {
            return;
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

    private static bool IsFlowFieldWithOptionCalculation(FieldSyntax fieldSyntax, SyntaxNodeAnalysisContext ctx)
    {
        var propertyList = fieldSyntax.PropertyList?.Properties;
        if (propertyList is null)
            return false;

        foreach (var property in propertyList)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            if (property is PropertySyntax propertySyntax &&
                propertySyntax.Value is FieldCalculationFormulaSyntax fieldCalculation &&
                fieldCalculation.Field is QualifiedNameSyntax qualifiedNameSyntax)
            {
                var fieldSymbol = ctx.SemanticModel.GetSymbolInfo(qualifiedNameSyntax, ctx.CancellationToken).Symbol;
                if (fieldSymbol?.GetTypeSymbol().GetNavTypeKindSafe() == NavTypeKind.Option)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region VariableAnalysis
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

                var localVariablesName = GetReferencedVariableNames(containingSymbol, variable);
                var localVariables = ((IMethodSymbol)containingSymbol.OriginalDefinition).LocalVariables;
                HasVariablesNotInSource = HasReferencedVariablesNotInSource(localVariables, localVariablesName);
                break;

            case SymbolKind.GlobalVariable:
                IApplicationObjectTypeSymbol? applicationObjectTypeSymbol = variable.GetContainingApplicationObjectTypeSymbol();
                if (applicationObjectTypeSymbol is null)
                    return;

                var globalVariablesName = GetReferencedVariableNames(applicationObjectTypeSymbol, variable);
                var globalVariables = GetGlobalVariablesFromApplicationObject(applicationObjectTypeSymbol);
                HasVariablesNotInSource = HasReferencedVariablesNotInSource(globalVariables, globalVariablesName);
                break;
        }

        if (HasVariablesNotInSource is false)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0088AvoidOptionTypes,
                variable.GetLocation()
            ));
        }
    }

    private static bool HasReferencedVariablesNotInSource(
        IEnumerable<IVariableSymbol> variables,
        HashSet<string>? referencedNames)
    {
        if (referencedNames is null)
            return false;

        // Filter by name first (cheaper operation), then check location
        return variables
            .Where(var => referencedNames.Contains(var.OriginalDefinition.Name))
            .Any(var => !var.Type.GetLocation().IsInSource);
    }

    private static List<IVariableSymbol> GetGlobalVariablesFromApplicationObject(IApplicationObjectTypeSymbol applicationObjectTypeSymbol)
    {
        var variables = new List<IVariableSymbol>();

        foreach (var member in applicationObjectTypeSymbol.GetMembers())
        {
            if (member.Kind == SymbolKind.GlobalVariable && member is IVariableSymbol globalVar)
            {
                variables.Add(globalVar);
            }
            else if (member.Kind == SymbolKind.Method && member is IMethodSymbol method)
            {
                variables.AddRange(method.LocalVariables);
            }
        }

        return variables;
    }

    private static HashSet<string>? GetReferencedVariableNames(ISymbol? containingSymbol, IVariableSymbol variable)
    {
        SyntaxNode? syntaxNode = containingSymbol?.DeclaringSyntaxReference?.GetSyntax();
        if (syntaxNode is null)
            return null;

        var variableName = variable.Name;
        var referencedNames = new HashSet<string>();

        // Find all argument lists that reference the variable
        var argumentLists = syntaxNode.DescendantNodes()
            .OfType<ArgumentListSyntax>();

        foreach (var argList in argumentLists)
        {
            // Check if any argument in this list references our variable
            bool hasVariableReference = false;
            foreach (var argument in argList.Arguments)
            {
                if ((argument is OptionAccessExpressionSyntax optionAccess &&
                     optionAccess.Expression.GetIdentifierOrLiteralValue() == variableName) ||
                    argument.GetIdentifierOrLiteralValue() == variableName)
                {
                    hasVariableReference = true;
                    break;
                }
            }

            if (hasVariableReference)
            {
                // Get the member access expressions from the containing expression statement
                foreach (var exprStmt in argList.AncestorsAndSelf().OfType<ExpressionStatementSyntax>())
                {
                    foreach (var memberAccess in exprStmt.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
                    {
                        var expressionName = memberAccess.Expression.ToString().UnquoteIdentifier();
                        if (!string.IsNullOrEmpty(expressionName))
                        {
                            referencedNames.Add(expressionName);
                        }
                    }
                }
            }
        }

        return referencedNames;
    }
    #endregion
}