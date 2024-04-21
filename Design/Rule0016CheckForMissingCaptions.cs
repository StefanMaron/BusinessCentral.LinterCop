﻿using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    class Rule0016CheckForMissingCaptions : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0016CheckForMissingCaptions);

        private static readonly List<string> PromotedGroupNames = new List<string>
        {
            "category_new",
            "category_process",
            "category_report",
            "category_category4",
            "category_category5",
            "category_category6",
            "category_category7",
            "category_category8",
            "category_category9",
            "category_category10",
            "category_category11",
            "category_category12",
            "category_category13",
            "category_category14",
            "category_category15",
            "category_category16",
            "category_category17",
            "category_category18",
            "category_category19",
            "category_category20",
        };

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
            if (context.Symbol.IsObsoletePending || context.Symbol.IsObsoleteRemoved) return;
            if (context.Symbol.GetContainingObjectTypeSymbol().IsObsoletePending || context.Symbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;

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
#if Fall2022
            if (Symbol.GetEnumPropertyValue<ShowAsKind>(PropertyKind.ShowAs) == ShowAsKind.SplitButton)
                return false;
#endif
            if (SemanticFacts.IsSameName(Symbol.MostSpecificKind, "Group") && PromotedGroupNames.Contains(Symbol.Name.ToLowerInvariant()))
                return false;

            if (Symbol.GetBooleanPropertyValue(PropertyKind.ShowCaption) != false)
                if (Symbol.GetProperty(PropertyKind.Caption) == null && Symbol.GetProperty(PropertyKind.CaptionClass) == null)
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