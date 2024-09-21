#nullable disable // TODO: Enable nullable and review rule
using BusinessCentral.LinterCop.AnalysisContextExtension;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0016CheckForMissingCaptions : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0016CheckForMissingCaptions);

        private static readonly HashSet<string> _predefinedActionCategoryNames = SyntaxFacts.PredefinedActionCategoryNames.Select(x => x.Key.ToLowerInvariant()).ToHashSet();

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForMissingCaptions),
                SymbolKind.Page,
                SymbolKind.Query,
                SymbolKind.Table,
                SymbolKind.Field,
                SymbolKind.Action,
                SymbolKind.EnumValue,
                SymbolKind.Control,
                SymbolKind.PermissionSet
            );

        private void CheckForMissingCaptions(SymbolAnalysisContext context)
        {
            if (context.IsObsoletePendingOrRemoved()) return;

            if (context.Symbol.Kind == SymbolKind.Control)
            {
                var Control = (IControlSymbol)context.Symbol;
                switch (Control.ControlKind)
                {
                    case ControlKind.Field:
                        if (CaptionIsMissing(context.Symbol, context))
                            if (Control.RelatedFieldSymbol != null)
                            {
                                if (CaptionIsMissing(Control.RelatedFieldSymbol, context))
                                    RaiseCaptionWarning(context);
                            }
                            else
                            {
                                if (!SuppressCaptionWarning(context))
                                    RaiseCaptionWarning(context);
                            }
                        break;

                    case ControlKind.Area:
                        break;

                    case ControlKind.Grid:
                        break;

                    case ControlKind.Repeater:
                        break;

                    case ControlKind.Part:
                        if (CaptionIsMissing(context.Symbol, context))
                            if (Control.RelatedPartSymbol != null)
                                if (CaptionIsMissing(Control.RelatedPartSymbol, context))
                                    if (!SuppressCaptionWarning(context))
                                        RaiseCaptionWarning(context);
                        break;

                    case ControlKind.UserControl:
                        break;

                    case ControlKind.SystemPart:
                        break;

                    default:
                        if (CaptionIsMissing(context.Symbol, context))
                            RaiseCaptionWarning(context);
                        break;
                }
            }
            else if (context.Symbol is IActionSymbol actionSymbol)
            {
                switch (actionSymbol.ActionKind)
                {
                    case ActionKind.Action:
                        if (CaptionIsMissing(context.Symbol, context))
                            RaiseCaptionWarning(context);
                        break;

                    case ActionKind.Group:
                        if (context.Symbol.GetEnumPropertyValue<ShowAsKind>(PropertyKind.ShowAs) == ShowAsKind.SplitButton)
                        {
                            // There is one specifc case where a Caption is needed on a Group where the property ShowAs is set to SplitButton
                            // A) The group is inside a Promoted Area
                            // B) Has one or more actionrefs
                            // C) One of the actions of the actionsrefs has Scope set to Repeater

                            if (context.Symbol.ContainingSymbol is not IActionSymbol containingSymbol)
                                return;

                            if (containingSymbol.ActionKind != ActionKind.Area)
                                break;

                            if (!SemanticFacts.IsSameName(context.Symbol.ContainingSymbol.Name, "Promoted"))
                                break;

                            if (!actionSymbol.Actions.Where(a => a.ActionKind == ActionKind.ActionRef)
                                                     .Where(a => a.Target.GetEnumPropertyValueOrDefault<PageActionScopeKind>(PropertyKind.Scope) == PageActionScopeKind.Repeater)
                                                     .Any())
                                break;

                            if (CaptionIsMissing(context.Symbol, context))
                                RaiseCaptionWarning(context);
                            break;
                        }
                        else
                        {
                            if (CaptionIsMissing(context.Symbol, context))
                                RaiseCaptionWarning(context);
                            break;
                        }
                }
            }
            else if (context.Symbol.Kind == SymbolKind.EnumValue)
            {
                IEnumValueSymbol enumValueSymbol = (IEnumValueSymbol)context.Symbol;
                if (enumValueSymbol.Name != "" && CaptionIsMissing(context.Symbol, context))
                    RaiseCaptionWarning(context);
            }
            else if (context.Symbol.Kind == SymbolKind.Page)
            {
                if (((IPageTypeSymbol)context.Symbol).PageType != PageTypeKind.API)
                    if (CaptionIsMissing(context.Symbol, context))
                        RaiseCaptionWarning(context);
            }
            else if (context.Symbol.Kind == SymbolKind.PermissionSet)
            {
                IPropertySymbol assignableProperty = context.Symbol.GetProperty(PropertyKind.Assignable);
                if (assignableProperty == null || (bool)assignableProperty.Value)
                    if (CaptionIsMissing(context.Symbol, context))
                        RaiseCaptionWarning(context);
            }
            else
            {
                if (CaptionIsMissing(context.Symbol, context))
                    RaiseCaptionWarning(context);
            }
        }

        private bool CaptionIsMissing(ISymbol Symbol, SymbolAnalysisContext context)
        {
            if (Symbol.ContainingType?.Kind == SymbolKind.Table)
            {
                if (((ITableTypeSymbol)Symbol.ContainingType).Id >= 2000000000)
                    return false;
                if (((IFieldSymbol)Symbol).Id >= 2000000000)
                    return false;
            }

            if (Symbol.Kind == SymbolKind.Action && ((IActionSymbol)Symbol).ActionKind == ActionKind.Group && _predefinedActionCategoryNames.Contains(Symbol.Name.ToLowerInvariant()))
                return false;

            if (Symbol.GetBooleanPropertyValue(PropertyKind.ShowCaption) != false)
                if (Symbol.GetProperty(PropertyKind.Caption) == null && Symbol.GetProperty(PropertyKind.CaptionClass) == null && Symbol.GetProperty(PropertyKind.CaptionML) == null)
                    return true;
            return false;
        }

        private static bool SuppressCaptionWarning(SymbolAnalysisContext context)
        {
            if (context.Symbol.GetContainingObjectTypeSymbol().GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Page) return false;
            IPageTypeSymbol pageTypeSymbol = (IPageTypeSymbol)context.Symbol.GetContainingObjectTypeSymbol();
            if (pageTypeSymbol.GetNavTypeKindSafe() != NavTypeKind.Page || pageTypeSymbol.PageType != PageTypeKind.API) return false;
            LinterSettings.Create(context.Compilation.FileSystem.GetDirectoryPath());
            return !LinterSettings.instance.enableRule0016ForApiObjects;
        }

        private void RaiseCaptionWarning(SymbolAnalysisContext context)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0016CheckForMissingCaptions, context.Symbol.GetLocation()));
        }
    }
}
