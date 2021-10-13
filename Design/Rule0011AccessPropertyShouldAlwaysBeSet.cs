using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

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
            if (context.Symbol.GetProperty(PropertyKind.Access) == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet, context.Symbol.GetLocation()));
            }
        }
    }
}
