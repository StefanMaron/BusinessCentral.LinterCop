using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0041EmptyCaptionLocked : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0041EmptyCaptionLocked);

        // List based on https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/properties/devenv-caption-property
        public override void Initialize(AnalysisContext context)
            => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(AnalyzeCaptionProperty), new SyntaxKind[] {
                SyntaxKind.TableObject,
                SyntaxKind.Field, // TableField
                SyntaxKind.PageField,
                SyntaxKind.PageGroup,
                SyntaxKind.PageObject,
                SyntaxKind.RequestPage,
                SyntaxKind.PageLabel,
                SyntaxKind.PageGroup,
                SyntaxKind.PagePart,
                SyntaxKind.PageSystemPart,
                SyntaxKind.PageAction,
                SyntaxKind.PageActionSeparator,
                SyntaxKind.PageActionGroup,
                SyntaxKind.XmlPortObject,
                SyntaxKind.ReportObject,
                SyntaxKind.QueryObject,
                SyntaxKind.QueryColumn,
                SyntaxKind.QueryFilter,
                SyntaxKind.ReportColumn,
                SyntaxKind.EnumValue,
                SyntaxKind.PageCustomAction,
                SyntaxKind.PageSystemAction,
                SyntaxKind.PageView,
                SyntaxKind.ReportLayout,
                SyntaxKind.ProfileObject,
                SyntaxKind.EnumType,
                SyntaxKind.PermissionSet,
                SyntaxKind.TableExtensionObject,
                SyntaxKind.PageExtensionObject
            });

        private void AnalyzeCaptionProperty(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            if (ctx.Node.IsKind(SyntaxKind.EnumValue) && ctx.ContainingSymbol.Kind == SymbolKind.Enum) return; // Prevent double raising the rule on EnumValue in a EnumObject

            LabelPropertyValueSyntax captionProperty = ctx.Node?.GetProperty("Caption")?.Value as LabelPropertyValueSyntax;
            if (captionProperty?.Value.LabelText.GetLiteralValue() == null || captionProperty.Value.LabelText.GetLiteralValue().ToString().Trim() != "") return;

            if (captionProperty.Value.Properties?.Values.Where(prop => prop.Identifier.Text.ToLowerInvariant() == "locked").FirstOrDefault() != null) return;

            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0041EmptyCaptionLocked, captionProperty.GetLocation()));
        }
    }
}