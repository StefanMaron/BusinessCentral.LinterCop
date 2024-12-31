using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0023AlwaysSpecifyFieldgroups : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0023AlwaysSpecifyFieldgroups);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckFieldgroups), SymbolKind.Table);

    private void CheckFieldgroups(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not ITableTypeSymbol table)
            return;

        if (IsTableOfTypeSetupTable(table))
            return;

        var fieldGroupLocation = GetFieldGroupLocation(ctx, table);

        CheckFieldGroup(ctx, table, "Brick", fieldGroupLocation);
        CheckFieldGroup(ctx, table, "DropDown", fieldGroupLocation);
    }

    private static Location GetFieldGroupLocation(SymbolAnalysisContext ctx, ITableTypeSymbol table)
    {
        var location = table.GetLocation();

        if (ctx.Symbol.DeclaringSyntaxReference?.GetSyntax(ctx.CancellationToken) is not TableSyntax tableSyntax)
            return location;

        if (tableSyntax.FieldGroups is not null)
        {
            var fieldGroupNode = tableSyntax.FieldGroups
                  .ChildNodesAndTokens()
                  .FirstOrDefault(node => node.Kind == SyntaxKind.FieldGroupsKeyword);

            var fieldGroupNodeLocation = fieldGroupNode.GetLocation();
            if (fieldGroupNodeLocation is not null)
                return fieldGroupNode.GetLocation()!;
        }

        if (tableSyntax.Keys is not null && table.GetLocation().SourceTree is SyntaxTree sourceTree)
        {
            var startPos = tableSyntax.Keys.Span.End + 2; // Should result in the blank line right after the keys section
            return Location.Create(sourceTree, new TextSpan(startPos, 1));
        }

        return location;
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
}