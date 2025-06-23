using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0023AlwaysSpecifyFieldgroups : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(compilationContext =>
        {
            BuildGlobalPageTableMapping(compilationContext);
            compilationContext.RegisterSymbolAction(symbolContext =>
            {
                CheckFieldgroups(symbolContext);
            }, SymbolKind.Table);
        });
    }

    private void CheckFieldgroups(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not ITableTypeSymbol table)
            return;

        if (IsTableOfTypeSetupTable(table))
            return;

        if (IsTemporaryTable(table) && !HasRelatedPageObjects(table))
            return;

        CheckFieldGroup(ctx, table, "Brick", table.GetLocation());
        CheckFieldGroup(ctx, table, "DropDown", table.GetLocation());
    }

    private static void CheckFieldGroup(SymbolAnalysisContext ctx, ITableTypeSymbol table, string fieldGroupName, Location location)
    {
        if (!table.FieldGroups.Any(item => item.Name == fieldGroupName && item.Fields.Length > 0))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups,
                location,
                fieldGroupName,
                table.Name));
        }
    }

    private static bool IsTableOfTypeSetupTable(ITableTypeSymbol table)
    {
        // Expect Primary Key to contains only one field
        if (table.PrimaryKey is null || table.PrimaryKey.Fields.Length != 1)
            return false;

        // The field should be of type Code
        if (table.PrimaryKey.Fields[0].GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Code)
            return false;

        // The field should be exactly (case sensitive) called 'Primary Key'
        if (table.PrimaryKey.Fields[0].Name != "Primary Key")
            return false;

        return true;
    }

    private static bool IsTemporaryTable(ITableTypeSymbol table)
        => table.TableType == TableTypeKind.Temporary;

    private bool HasRelatedPageObjects(ITableTypeSymbol table)
        => GlobalPageTableMapping.ContainsKey(table);

    private Dictionary<ITableTypeSymbol, List<IPageTypeSymbol>> GlobalPageTableMapping = new();
    private void BuildGlobalPageTableMapping(CompilationStartAnalysisContext context)
    {
        GlobalPageTableMapping.Clear();

        foreach (var tree in context.Compilation.SyntaxTrees)
        {
            var root = tree.GetRoot(context.CancellationToken);
            var semanticModel = context.Compilation.GetSemanticModel(tree);

            foreach (var pageDeclaration in root.DescendantNodes().OfType<PageSyntax>())
            {
                var pageSymbol = semanticModel.GetDeclaredSymbol(pageDeclaration, context.CancellationToken) as IPageTypeSymbol;
                if (pageSymbol is null || pageSymbol.RelatedTable is null)
                    continue;

                var tableSymbol = pageSymbol.RelatedTable;
                if (!GlobalPageTableMapping.TryGetValue(tableSymbol, out var pageList))
                {
                    pageList = new List<IPageTypeSymbol>();
                    GlobalPageTableMapping[tableSymbol] = pageList;
                }

                if (!pageList.Contains(pageSymbol))
                    pageList.Add(pageSymbol);
            }
        }
    }
}