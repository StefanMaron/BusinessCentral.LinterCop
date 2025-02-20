using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0039ArgumentDifferentTypeThenExpected : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0039ArgumentDifferentTypeThenExpected, DiagnosticDescriptors.Rule0049PageWithoutSourceTable, DiagnosticDescriptors.Rule0058PageVariableMethodOnTemporaryTable);

    internal static readonly ImmutableHashSet<string> pageProcedureNames = (new string[4]
    {
        "GetRecord",
        "SetRecord",
        "SetSelectionFilter",
        "SetTableView",
    }).ToImmutableHashSet<string>();

    internal static readonly ImmutableHashSet<string> pageRunProcedureNames = (new string[2]
    {
        "Run",
        "RunModal",
    }).ToImmutableHashSet<string>();

    private static readonly List<PropertyKind> referencePageProviders = new List<PropertyKind>
        {
            PropertyKind.LookupPageId,
            PropertyKind.DrillDownPageId
        };

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeRunPageArguments), OperationKind.InvocationExpression);
        context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeSetRecordArgument), OperationKind.InvocationExpression);
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeTableReferencePageProvider), SymbolKind.Table);
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeTableExtensionReferencePageProvider), SymbolKind.TableExtension);
    }

    private void AnalyzeRunPageArguments(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            !pageRunProcedureNames.Contains(operation.TargetMethod.Name) ||
            operation.Arguments.Length < 2)
            return;

        if (operation.Arguments[0].Syntax.Kind != SyntaxKind.OptionAccessExpression)
            return;

        if (operation.Arguments[1].Syntax.Kind != SyntaxKind.IdentifierName || operation.Arguments[1].Value.Kind != OperationKind.ConversionExpression)
            return;

        if (operation.TargetMethod?.ContainingType?.GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Page)
            return;

        IApplicationObjectTypeSymbol applicationObjectTypeSymbol = ((IApplicationObjectAccess)operation.Arguments[0].Value).ApplicationObjectTypeSymbol;
        if (applicationObjectTypeSymbol.GetNavTypeKindSafe() != NavTypeKind.Page)
            return;

        ITableTypeSymbol? pageSourceTable = ((IPageTypeSymbol)applicationObjectTypeSymbol.GetTypeSymbol()).RelatedTable;
        if (pageSourceTable is null)
            return;

        IOperation operand = ((IConversionExpression)operation.Arguments[1].Value).Operand;
        if (operand.GetSymbol()?.GetTypeSymbol() is not IRecordTypeSymbol recordTypeSymbol) return;
        ITableTypeSymbol recordArgument = recordTypeSymbol.BaseTable;

        if (!AreTheSameNavObjects(recordArgument, pageSourceTable))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0039ArgumentDifferentTypeThenExpected,
                ctx.Operation.Syntax.GetLocation(),
                2,
                operand.GetSymbol()!.GetTypeSymbol().ToString(), pageSourceTable.GetNavTypeKindSafe() + pageSourceTable.Name.QuoteIdentifierIfNeeded()));
        }
    }

    private void AnalyzeSetRecordArgument(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod)
            return;

        if (operation?.TargetMethod?.ContainingType?.GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Page)
            return;

        if (!pageProcedureNames.Contains(operation.TargetMethod.Name))
            return;

        if (operation.Arguments.Length != 1)
            return;

        if (operation.Arguments[0].Syntax.Kind != SyntaxKind.IdentifierName || operation.Arguments[0].Value.Kind != OperationKind.ConversionExpression) return;

        IOperation pageReference = ctx.Operation.DescendantsAndSelf().Where(x => x.GetSymbol() is not null)
                                                    .Where(x => x.Type.GetNavTypeKindSafe() == NavTypeKind.Page)
                                                    .SingleOrDefault();
        if (pageReference is null)
            return;

        ISymbol? variableSymbol = pageReference.GetSymbol()?.OriginalDefinition;
        if (variableSymbol is null)
            return;

        IPageTypeSymbol pageTypeSymbol = (IPageTypeSymbol)variableSymbol.GetTypeSymbol().OriginalDefinition;
        if (pageTypeSymbol.RelatedTable is null)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0049PageWithoutSourceTable,
                ctx.Operation.Syntax.GetLocation(),
                NavTypeKind.Page,
                GetFullyQualifiedObjectName(pageTypeSymbol)));

            return;
        }

        IOperation operand = ((IConversionExpression)operation.Arguments[0].Value).Operand;
        IRecordTypeSymbol? recordTypeSymbol = operand.GetSymbol()?.GetTypeSymbol() as IRecordTypeSymbol;
        if (recordTypeSymbol is null) return;
        if (recordTypeSymbol.Temporary && SemanticFacts.IsSameName(operation.TargetMethod.Name, "SetRecord"))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0058PageVariableMethodOnTemporaryTable,
                ctx.Operation.Syntax.GetLocation(),
                variableSymbol.ToString().QuoteIdentifierIfNeeded(),
                operation.TargetMethod.Name));

            return;
        }
        ITableTypeSymbol pageSourceTable = pageTypeSymbol.RelatedTable;
        ITableTypeSymbol recordArgument = recordTypeSymbol.BaseTable;

        if (!AreTheSameNavObjects(recordArgument, pageSourceTable))
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0039ArgumentDifferentTypeThenExpected,
                ctx.Operation.Syntax.GetLocation(),
                1,
                operand.GetSymbol()!.GetTypeSymbol().ToString(),
                pageSourceTable.GetNavTypeKindSafe().ToString() + ' ' + pageSourceTable.Name.QuoteIdentifierIfNeeded()
                ));
    }

    private void AnalyzeTableReferencePageProvider(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not ITableTypeSymbol table)
            return;

        foreach (PropertyKind propertyKind in referencePageProviders)
        {
            IPropertySymbol? pageReference = table.GetProperty(propertyKind);
            if (pageReference is null)
                continue;

            IPageTypeSymbol page = (IPageTypeSymbol)pageReference.Value;
            if (page is null)
                continue;

            ITableTypeSymbol? pageSourceTable = page.RelatedTable;
            if (pageSourceTable is null)
                continue;

            if (!AreTheSameNavObjects(table, pageSourceTable))
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0039ArgumentDifferentTypeThenExpected,
                    pageReference.GetLocation(),
                    1,
                    table.GetTypeSymbol().GetNavTypeKindSafe().ToString() + ' ' + table.Name.QuoteIdentifierIfNeeded(),
                    pageSourceTable.GetNavTypeKindSafe().ToString() + ' ' + pageSourceTable.Name.QuoteIdentifierIfNeeded()));
        }
    }

    private void AnalyzeTableExtensionReferencePageProvider(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not ITableExtensionTypeSymbol tableExtension)
            return;

        if (tableExtension.Target is not ITableTypeSymbol table)
            return;

        foreach (PropertyKind propertyKind in referencePageProviders)
        {
            IPropertySymbol? pageReference = tableExtension.GetProperty(propertyKind);
            if (pageReference is null)
                continue;

            IPageTypeSymbol page = (IPageTypeSymbol)pageReference.Value;
            ITableTypeSymbol? pageSourceTable = page.RelatedTable;
            if (pageSourceTable is null)
                continue;

            if (!AreTheSameNavObjects(table, pageSourceTable))
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0039ArgumentDifferentTypeThenExpected,
                    pageReference.GetLocation(),
                    1,
                    table.GetTypeSymbol().GetNavTypeKindSafe().ToString() + ' ' + table.Name.QuoteIdentifierIfNeeded(),
                    pageSourceTable.GetNavTypeKindSafe().ToString() + ' ' + pageSourceTable.Name.QuoteIdentifierIfNeeded()));
        }
    }

    private static bool AreTheSameNavObjects(ITableTypeSymbol left, ITableTypeSymbol right)
    {
        if (left.GetNavTypeKindSafe() != right.GetNavTypeKindSafe())
            return false;

#if !LessThenFall2023RV1
        if (left.ContainingSymbol is not INamespaceSymbol leftNamespaceSymbol ||
            right.ContainingSymbol is not INamespaceSymbol rightNamespaceSymbol ||
            leftNamespaceSymbol.QualifiedName != rightNamespaceSymbol.QualifiedName)
            return false;
#endif
        if (left.Name != right.Name)
            return false;

        return true;
    }

    private static string GetFullyQualifiedObjectName(IPageTypeSymbol page)
    {
#if !LessThenFall2023RV1
        if (page.ContainingNamespace?.QualifiedName != "")
            return page.ContainingNamespace?.QualifiedName + "." + page.Name.QuoteIdentifierIfNeeded();
#endif
        return page.Name.QuoteIdentifierIfNeeded();
    }
}