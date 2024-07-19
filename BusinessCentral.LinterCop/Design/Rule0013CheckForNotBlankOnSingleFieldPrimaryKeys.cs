using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys, DiagnosticDescriptors.Rule0067DisableNotBlankOnSingleFieldPrimaryKey);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForSingleFieldPrimaryKeyNotBlankProperty), SymbolKind.Field);
    }
    private void CheckForSingleFieldPrimaryKeyNotBlankProperty(SymbolAnalysisContext context)
    {
        if (context.IsObsoletePendingOrRemoved()) return;

        IFieldSymbol field = (IFieldSymbol)context.Symbol;
        if (GetExitCondition(field))
            return;

        ITableTypeSymbol table = (ITableTypeSymbol)field.GetContainingObjectTypeSymbol();
        if (table.PrimaryKey.Fields.Length != 1)
            return;

        if (!table.PrimaryKey.Fields[0].Equals(field))
            return;

        if (TableContainsNoSeries(table))
        {
            if (field.GetBooleanPropertyValue(PropertyKind.NotBlank).GetValueOrDefault() && !SemanticFacts.IsSameName(field.Name, "Name"))
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0067DisableNotBlankOnSingleFieldPrimaryKey, field.GetLocation()));
        }
        else
        {
            if (field.GetProperty(PropertyKind.NotBlank) == null)
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys, field.GetLocation()));
        }
    }

    private static bool GetExitCondition(IFieldSymbol field)
    {
        return
            field.FieldClass != FieldClassKind.Normal ||
            field.GetContainingObjectTypeSymbol().Kind != SymbolKind.Table ||
            !field.DeclaringSyntaxReference.GetSyntax().DescendantNodes().Any(Token => Token.Kind == SyntaxKind.LengthDataType);
    }

    private static bool TableContainsNoSeries(ITableTypeSymbol table)
    {
        return table.Fields
            .Where(x => x.Id > 0 && x.Id < 2000000000)
            .Where(x => x.FieldClass == FieldClassKind.Normal)
#if Fall2024
            .Where(x => x.Type.GetNavTypeKindSafe() == NavTypeKind.Code)
#endif
            .Any(field =>
        {
            IPropertySymbol propertySymbol = field.GetProperty(PropertyKind.TableRelation);
            if (propertySymbol != null && propertySymbol.ContainingSymbol != null)
                return SemanticFacts.IsSameName(propertySymbol.ContainingSymbol.Name.UnquoteIdentifier(), "No. Series");
            return false;
        });
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0013CheckForNotBlankOnSingleFieldPrimaryKeys = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0013",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0013CheckForNotBlankOnSingleFieldPrimaryKeysTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0013CheckForNotBlankOnSingleFieldPrimaryKeysFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0013CheckForNotBlankOnSingleFieldPrimaryKeysDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0013");

        public static readonly DiagnosticDescriptor Rule0067DisableNotBlankOnSingleFieldPrimaryKey = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0067",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0067DisableNotBlankOnSingleFieldPrimaryKeyTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0067DisableNotBlankOnSingleFieldPrimaryKeyFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0067DisableNotBlankOnSingleFieldPrimaryKeyDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0067");
    }
}