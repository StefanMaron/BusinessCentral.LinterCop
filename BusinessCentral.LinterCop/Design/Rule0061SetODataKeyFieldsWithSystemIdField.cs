using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0061SetODataKeyFieldsWithSystemIdField : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0061SetODataKeyFieldsWithSystemIdField);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeODataKeyFieldsPropertyOnApiPage), SymbolKind.Page);

        private void AnalyzeODataKeyFieldsPropertyOnApiPage(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            if (ctx.Symbol is not IPageTypeSymbol pageTypeSymbol)
                return;

            if (pageTypeSymbol.PageType != PageTypeKind.API)
                return;

            if (pageTypeSymbol.GetBooleanPropertyValue(PropertyKind.SourceTableTemporary).GetValueOrDefault())
                return;

            IPropertySymbol property = pageTypeSymbol.GetProperty(PropertyKind.ODataKeyFields);

            // Set the location of the diagnostic on the property itself (if exists)
            Location location = pageTypeSymbol.GetLocation();
            if (property != null)
                location = property.GetLocation();

            if (property == null || property.Value == null || property.ValueText != "2000000000")
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0061SetODataKeyFieldsWithSystemIdField, location));
        }
    }
}