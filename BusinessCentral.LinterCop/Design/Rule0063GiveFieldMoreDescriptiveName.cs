using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0063GiveFieldMoreDescriptiveName : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0063GiveFieldMoreDescriptiveName);
        private static readonly Dictionary<string, string> _descriptiveNames = new Dictionary<string, string>
            {
                { "SystemId", "id" },
                { "Name", "displayName" },
                { "SystemModifiedAt", "lastModifiedDateTime" }
            };

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzePropertyApplicationAreaOnFieldsOfApiPage), SymbolKind.Page);

        private void AnalyzePropertyApplicationAreaOnFieldsOfApiPage(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            if (ctx.Symbol is not IPageTypeSymbol pageTypeSymbol)
                return;

            if (pageTypeSymbol.PageType != PageTypeKind.API)
                return;

            IEnumerable<IControlSymbol> pageFields = pageTypeSymbol.FlattenedControls
                                                            .Where(e => e.ControlKind == ControlKind.Field)
                                                            .Where(e => e.RelatedFieldSymbol != null);

            foreach (IControlSymbol field in pageFields)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();
                string descriptiveName = GetDescriptiveName(field);
                if (!string.IsNullOrEmpty(descriptiveName) && field.Name != descriptiveName)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0063GiveFieldMoreDescriptiveName, field.GetLocation(), new object[] { descriptiveName }));
                }
            }
        }

        private static string GetDescriptiveName(IControlSymbol field)
        {
            if (_descriptiveNames.ContainsKey(field.RelatedFieldSymbol.Name))
                return _descriptiveNames[field.RelatedFieldSymbol.Name];

            if (field.RelatedFieldSymbol.Name.Contains("No.")
                    && field.Name.Contains("no", StringComparison.OrdinalIgnoreCase)
                    && !field.Name.Contains("number", StringComparison.OrdinalIgnoreCase))
                return ReplaceNoWithNumber(field.Name);

            return null;
        }
        public static string ReplaceNoWithNumber(string input)
        {
            input = input.Replace("No", "Number");
            input = input.Replace("no", "number");
            return input;
        }
    }
}
