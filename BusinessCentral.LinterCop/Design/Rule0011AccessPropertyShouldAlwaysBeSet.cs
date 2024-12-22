using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0011DataPerCompanyShouldAlwaysBeSet : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet);

    public override VersionCompatibility SupportedVersions => VersionCompatibility.Spring2021OrGreater;

    public override void Initialize(AnalysisContext context) =>
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForMissingAccessProperty),
            SymbolKind.Codeunit,
            SymbolKind.Enum,
            SymbolKind.Interface,
            SymbolKind.PermissionSet,
            SymbolKind.Query,
            SymbolKind.Table,
            SymbolKind.Field);

    private void CheckForMissingAccessProperty(SymbolAnalysisContext ctx)
    {
        if (ctx.Symbol.Kind == SymbolKind.Enum || ctx.Symbol.Kind == SymbolKind.Interface)
            return;

        if (ctx.IsObsoletePendingOrRemoved())
            return;

        if (ctx.Symbol.Kind == SymbolKind.Field)
        {
            if (ctx.Symbol.ContainingSymbol is null || ctx.Symbol.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePendingOrRemoved())
                return;

            LinterSettings.Create(ctx.Compilation.FileSystem?.GetDirectoryPath());

            if (LinterSettings.instance.enableRule0011ForTableFields)
            {
                if (ctx.Symbol.GetProperty(PropertyKind.Access) is null)
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet,
                        ctx.Symbol.GetLocation()));
            }
        }
        else
        {
            if (ctx.Symbol.GetProperty(PropertyKind.Access) is null)
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet,
                    ctx.Symbol.GetLocation()));
        }
    }
}