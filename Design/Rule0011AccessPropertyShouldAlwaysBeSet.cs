using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0011DataPerCompanyShouldAlwaysBeSet : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForMissingAccessProperty), SymbolKind.Codeunit, SymbolKind.Enum, SymbolKind.Interface, SymbolKind.Page, SymbolKind.PermissionSet, SymbolKind.Query, SymbolKind.Table, SymbolKind.XmlPort);

        private void CheckForMissingAccessProperty(SymbolAnalysisContext context)
        {
            IApplicationObjectTypeSymbol applicationObject = (IApplicationObjectTypeSymbol)context.Symbol;

            if (applicationObject.GetProperty(PropertyKind.Access) == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0011AccessPropertyShouldAlwaysBeSet, applicationObject.GetLocation()));
            }
        }
    }
}
