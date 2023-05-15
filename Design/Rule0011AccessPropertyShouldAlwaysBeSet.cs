using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0011DataPerCompanyShouldAlwaysBeSet : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForMissingAccessProperty), SymbolKind.Codeunit, SymbolKind.Enum, SymbolKind.Interface, SymbolKind.PermissionSet, SymbolKind.Query, SymbolKind.Table, SymbolKind.Field);

        private void CheckForMissingAccessProperty(SymbolAnalysisContext context)
        {
            var manifest = AppSourceCopConfigurationProvider.GetManifest(context.Compilation);
            if (manifest.Runtime < RuntimeVersion.Spring2021 && (context.Symbol.Kind == SymbolKind.Enum || context.Symbol.Kind == SymbolKind.Interface))
                return;

            if (context.Symbol.IsObsoletePending || context.Symbol.IsObsoleteRemoved) return;
            if (context.Symbol.Kind == SymbolKind.Field)
            {
                if (context.Symbol.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || context.Symbol.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
                LinterSettings.Create(context.Compilation.FileSystem.GetDirectoryPath());
                if (LinterSettings.instance.enableRule0011ForTableFields)
                {
                    if (context.Symbol.GetProperty(PropertyKind.Access) == null)
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet, context.Symbol.GetLocation()));
                }
            }
            else
            {
                if (context.Symbol.GetProperty(PropertyKind.Access) == null)
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet, context.Symbol.GetLocation()));
            }

        }
    }
}
