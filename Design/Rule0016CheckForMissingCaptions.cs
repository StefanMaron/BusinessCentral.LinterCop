using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    class Rule0016CheckForMissingCaptions : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0016CheckForMissingCaptions);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForMissingCaptions),
                SymbolKind.Page,
                SymbolKind.Query,
                SymbolKind.Table,
                SymbolKind.Field,
                SymbolKind.Action,
                SymbolKind.EnumValue,
                SymbolKind.Control
            );

        private void CheckForMissingCaptions(SymbolAnalysisContext context)
        {
            if (context.Symbol.IsObsoletePending || context.Symbol.IsObsoleteRemoved) return;
            if (context.Symbol.GetContainingObjectTypeSymbol().IsObsoletePending || context.Symbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;

            if (context.Symbol.Kind == SymbolKind.Control)
            {
                var Control = ((IControlSymbol)context.Symbol);
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
                                if (IsPageTypeKindAPI(context))
                                {
                                    LinterSettings.Create();
                                    if (LinterSettings.instance.enableRule0016ForApiObjects)
                                        RaiseCaptionWarning(context);
                                }
                            }
                        break;

                    case ControlKind.Area:
                        break;

                    case ControlKind.Repeater:
                        break;

                    case ControlKind.Part:
                        if (CaptionIsMissing(context.Symbol, context))
                            if (Control.RelatedPartSymbol != null)
                                if (CaptionIsMissing(Control.RelatedPartSymbol, context))
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
            else if (context.Symbol.Kind == SymbolKind.Action)
            {
                switch (((IActionSymbol)context.Symbol).ActionKind)
                {
                    case ActionKind.Action:
                        if (CaptionIsMissing(context.Symbol, context))
                            RaiseCaptionWarning(context);
                        break;

                    case ActionKind.Group:
                        if (CaptionIsMissing(context.Symbol, context))
                            RaiseCaptionWarning(context);
                        break;
                }
            }
            else if (context.Symbol.Kind == SymbolKind.Page)
            {
                if (((IPageTypeSymbol)context.Symbol).PageType != PageTypeKind.API)
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
            try
            {
                if (Symbol.ContainingType.Kind == SymbolKind.Table)
                {
                    if (((ITableTypeSymbol)Symbol.ContainingType).Id >= 2000000000)
                        return false;
                    if (((IFieldSymbol)Symbol).Id >= 2000000000)
                        return false;
                }
            }
            catch (NullReferenceException)
            { }

            if (Symbol.GetEnumPropertyValue<ShowAsKind>(PropertyKind.ShowAs) == ShowAsKind.SplitButton)
                return false;

            if (Symbol.GetBooleanPropertyValue(PropertyKind.ShowCaption) != false)
                if (Symbol.GetProperty(PropertyKind.Caption) == null && Symbol.GetProperty(PropertyKind.CaptionClass) == null)
                    return true;
            return false;

        }
        private void RaiseCaptionWarning(SymbolAnalysisContext context)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0016CheckForMissingCaptions, context.Symbol.GetLocation()));
        }

        private static bool IsPageTypeKindAPI(SymbolAnalysisContext context)
        {
            if (context.Symbol.GetContainingObjectTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Page) return false;
            foreach (IPropertySymbol property in context.Symbol.GetContainingObjectTypeSymbol().Properties)
            {
                IPageTypeSymbol pageType = (IPageTypeSymbol)property.ContainingType;

                if (pageType.PageType == PageTypeKind.API) return true;
            }
            return false;
        }
    }
}