using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;
[DiagnosticAnalyzer]
public class Rule0076TableRelationTooLong : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0076TableRelationTooLong);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);

    private void AnalyzeSymbol(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IFieldSymbol field)
            return;

        if (!field.HasLength)
            return;

        var tableRelation = field
            .GetProperty(PropertyKind.TableRelation)
            ?.GetPropertyValueSyntax<TableRelationPropertyValueSyntax>();
        if (tableRelation is null)
            return;

        AnalyzeTableRelations(ctx, field, tableRelation);
    }

    private void AnalyzeTableRelations(SymbolAnalysisContext ctx, IFieldSymbol field, TableRelationPropertyValueSyntax? tableRelation)
    {
        while (tableRelation is not null)
        {
            var relatedFieldSymbol = ResolveRelatedField(ctx, tableRelation);

            if (relatedFieldSymbol is not null && ShouldReportDiagnostic(field, relatedFieldSymbol))
            {
                ReportLengthMismatch(ctx, field, relatedFieldSymbol);
            }

            tableRelation = tableRelation.ElseExpression?.ElseTableRelationCondition;
        }
    }

    private static bool ShouldReportDiagnostic(IFieldSymbol currentField, IFieldSymbol relatedField) =>
        relatedField.HasLength && currentField.Length < relatedField.Length;

    private static void ReportLengthMismatch(SymbolAnalysisContext ctx, IFieldSymbol currentField, IFieldSymbol relatedField)
    {
        ctx.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0076TableRelationTooLong,
            currentField.GetLocation(),
            relatedField.Length,
            relatedField.ToDisplayString().QuoteIdentifierIfNeeded(),
            currentField.Length,
            currentField.ToDisplayString().QuoteIdentifierIfNeeded()));
    }

    private IFieldSymbol? ResolveRelatedField(SymbolAnalysisContext ctx, TableRelationPropertyValueSyntax tableRelation)
    {
        return tableRelation.RelatedTableField switch
        {
            QualifiedNameSyntax qualifiedName =>
                ResolveQualifiedField(qualifiedName, ctx.Compilation),

            IdentifierNameSyntax identifierName =>
                ResolvePrimaryKeyField(identifierName.Identifier.ValueText, ctx.Compilation),

            _ => null
        };
    }

    private IFieldSymbol? ResolveQualifiedField(QualifiedNameSyntax qualifiedName, Compilation compilation)
    {
        if (qualifiedName.Left is IdentifierNameSyntax tableNameSyntax &&
            qualifiedName.Right is IdentifierNameSyntax fieldNameSyntax)
        {
            var tableName = tableNameSyntax.GetIdentifierOrLiteralValue();
            var fieldName = fieldNameSyntax.GetIdentifierOrLiteralValue();

            if (tableName != null && fieldName != null)
            {
                return GetFieldFromTable(tableName, fieldName, compilation)
                    ?? GetFieldFromTableExtension(tableName, fieldName, compilation);
            }
        }

        return null;
    }

    private static IFieldSymbol? ResolvePrimaryKeyField(string? tableName, Compilation compilation)
    {
        if (string.IsNullOrEmpty(tableName))
            return null;

        var tableSymbols = compilation.GetApplicationObjectTypeSymbolsByNameAcrossModules(SymbolKind.Table, tableName);

        return tableSymbols.FirstOrDefault() is ITableTypeSymbol table && table.PrimaryKey.Fields.Length == 1
            ? table.PrimaryKey.Fields[0]
            : null;
    }

    private static IFieldSymbol? GetFieldFromTable(string tableName, string fieldName, Compilation compilation)
    {
        var tableSymbols = compilation.GetApplicationObjectTypeSymbolsByNameAcrossModules(SymbolKind.Table, tableName);

        return tableSymbols.FirstOrDefault() is ITableTypeSymbol table
            ? table.Fields.FirstOrDefault(f => f.Name == fieldName)
            : null;
    }

    private static IFieldSymbol? GetFieldFromTableExtension(string tableName, string fieldName, Compilation compilation)
    {
        return compilation.GetDeclaredApplicationObjectSymbols()
            .OfType<ITableExtensionTypeSymbol>()
            .Where(ext => ext.Target?.Name == tableName)
            .SelectMany(ext => ext.AddedFields)
            .FirstOrDefault(field => field.Name == fieldName);
    }
}