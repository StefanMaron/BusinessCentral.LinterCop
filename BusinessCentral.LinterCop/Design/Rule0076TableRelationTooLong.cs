﻿using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;
[DiagnosticAnalyzer]
public class Rule0076TableRelationTooLong : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0076TableRelationTooLong);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);

    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        if (context.IsObsoletePendingOrRemoved())
            return;

        if (context.Symbol is not IFieldSymbol currentField)
            return;

        var tableRelation = currentField
            .GetProperty(PropertyKind.TableRelation)
            ?.GetPropertyValueSyntax<TableRelationPropertyValueSyntax>();
            
        if (tableRelation is null)
            return;

        AnalyzeTableRelations(context, currentField, tableRelation);
    }

    private void AnalyzeTableRelations(SymbolAnalysisContext context, IFieldSymbol currentField, TableRelationPropertyValueSyntax? tableRelation)
    {
        while (tableRelation is not null)
        {
            if (tableRelation.RelatedTableField is QualifiedNameSyntax relatedField)
            {
                var relatedFieldSymbol = GetRelatedFieldSymbol(
                    relatedField.Left as IdentifierNameSyntax,
                    relatedField.Right as IdentifierNameSyntax,
                    context.Compilation);

                if (relatedFieldSymbol is not null && ShouldReportDiagnostic(currentField, relatedFieldSymbol))
                {
                    ReportLengthMismatch(context, currentField, relatedFieldSymbol, relatedField);
                }
            }

            tableRelation = tableRelation.ElseExpression?.ElseTableRelationCondition;
        }
    }

    private static bool ShouldReportDiagnostic(IFieldSymbol currentField, IFieldSymbol? relatedField) =>
        relatedField?.HasLength == true &&
        currentField.HasLength &&
        currentField.Length < relatedField.Length;

    private static void ReportLengthMismatch(SymbolAnalysisContext context, IFieldSymbol currentField,
        IFieldSymbol relatedField, QualifiedNameSyntax relatedFieldSyntax)
    {
        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.Rule0076TableRelationTooLong,
            currentField.GetLocation(),
            relatedField.Length,
            relatedFieldSyntax.ToString(),
            currentField.Length,
            currentField.Name));
    }

    private IFieldSymbol? GetRelatedFieldSymbol(IdentifierNameSyntax? table, IdentifierNameSyntax? field, Compilation compilation)
    {
        if (table?.GetIdentifierOrLiteralValue() is not string tableName ||
            field?.GetIdentifierOrLiteralValue() is not string fieldName)
            return null;

        return GetFieldFromTable(tableName, fieldName, compilation) ??
               GetFieldFromTableExtension(tableName, fieldName, compilation);
    }

    private static IFieldSymbol? GetFieldFromTable(string tableName, string fieldName, Compilation compilation)
    {
        var tables = compilation.GetApplicationObjectTypeSymbolsByNameAcrossModules(SymbolKind.Table, tableName);
        return tables.FirstOrDefault() is ITableTypeSymbol tableSymbol
            ? tableSymbol.Fields.FirstOrDefault(x => x.Name == fieldName)
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

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0076TableRelationTooLong = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0076",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0076TableRelationTooLongTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0076TableRelationTooLongFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0076TableRelationTooLongDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0076");
    }
}