using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0062MandatoryFieldMissingOnApiPage : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0062MandatoryFieldMissingOnApiPage);

        private static readonly Dictionary<string, string> MandatoryFields = new Dictionary<string, string>
            {
                { "SystemId", "id" },
                { "SystemModifiedAt", "lastModifiedDateTime" }
            };

        public override void Initialize(AnalysisContext context) =>
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeRule0062MandatoryFieldOnApiPage), SymbolKind.Page);

        private void AnalyzeRule0062MandatoryFieldOnApiPage(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IPageTypeSymbol pageTypeSymbol) return;

            if (pageTypeSymbol.PageType != PageTypeKind.API)
                return;

            if (pageTypeSymbol.GetBooleanPropertyValue(PropertyKind.SourceTableTemporary).GetValueOrDefault())
                return;

            IEnumerable<IControlSymbol> pageFields = pageTypeSymbol.FlattenedControls
                                                            .Where(e => e.ControlKind == ControlKind.Field && e.RelatedFieldSymbol is not null);

            IEnumerable<KeyValuePair<string, string>> missingMandatoryFields = MandatoryFields
                    .Where(mf => !pageFields.Any(pf => mf.Key == pf.RelatedFieldSymbol?.Name && mf.Value == pf.Name));

            foreach (KeyValuePair<string, string> field in missingMandatoryFields)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.Rule0062MandatoryFieldMissingOnApiPage,
                    pageTypeSymbol.GetLocation(),
                    field.Key,
                    field.Value));
            }
        }
    }
}