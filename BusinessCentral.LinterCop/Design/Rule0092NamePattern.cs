using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0092NamePattern : DiagnosticAnalyzer
{
    private Regex? _procedureNameAllowPattern = null;
    private Regex? _procedureNameDisallowPattern = null;
    private Regex? _procedureNameGlobalAllowPattern = null;
    private Regex? _procedureNameGlobalDisallowPattern = null;
    private Regex? _procedureNameLocalAllowPattern = null;
    private Regex? _procedureNameLocalDisallowPattern = null;
    private Regex? _eventSubscriberAllowPattern = null;
    private Regex? _eventSubscriberDisallowPattern = null;
    private Regex? _eventDeclarationAllowPattern = null;
    private Regex? _eventDeclarationDisallowPattern = null;
    private Regex? _variableAndParameterNameAllowPattern = null;
    private Regex? _variableAndParameterNameDisallowPattern = null;
    private Regex? _captionNameAllowPattern = null;
    private Regex? _captionNameDisallowPattern = null;
    private Regex? _fieldNameAllowPattern = null;
    private Regex? _fieldNameDisallowPattern = null;
    private Regex? _groupNameAllowPattern = null;
    private Regex? _groupNameDisallowPattern = null;
    private Regex? _actionNameAllowPattern = null;
    private Regex? _actionNameDisallowPattern = null;
    private Regex _apiPageFieldAllowPattern;

    public Rule0092NamePattern()
    {
        var procedureNameSettings = LinterSettings.instance?.procedureNamePattern;
        if (procedureNameSettings != null)
        {
            _procedureNameAllowPattern = Pattern.CompilePattern(procedureNameSettings.AllowPattern);
            _procedureNameDisallowPattern = Pattern.CompilePattern(procedureNameSettings.DisallowPattern);
            _procedureNameGlobalAllowPattern = Pattern.CompilePattern(procedureNameSettings.GlobalProcedureAllowPattern);
            _procedureNameGlobalDisallowPattern = Pattern.CompilePattern(procedureNameSettings.GlobalProcedureDisallowPattern);
            _procedureNameLocalAllowPattern = Pattern.CompilePattern(procedureNameSettings.LocalProcedureAllowPattern);
            _procedureNameLocalDisallowPattern = Pattern.CompilePattern(procedureNameSettings.LocalProcedureDisallowPattern);
            _eventSubscriberAllowPattern = Pattern.CompilePattern(procedureNameSettings.EventSubscriberAllowPattern);
            _eventSubscriberDisallowPattern = Pattern.CompilePattern(procedureNameSettings.EventSubscriberDisallowPattern);
            _eventDeclarationAllowPattern = Pattern.CompilePattern(procedureNameSettings.EventDeclarationAllowPattern);
            _eventDeclarationDisallowPattern = Pattern.CompilePattern(procedureNameSettings.EventDeclarationDisallowPattern);
        }

        var variableAndParameterNameSettings = LinterSettings.instance?.variableAndParameterNamePattern;
        if (variableAndParameterNameSettings != null)
        {
            _variableAndParameterNameAllowPattern = Pattern.CompilePattern(variableAndParameterNameSettings.AllowPattern);
            _variableAndParameterNameDisallowPattern = Pattern.CompilePattern(variableAndParameterNameSettings.DisallowPattern);
        }

        var captionNameSettings = LinterSettings.instance?.captionNamePattern;
        if (captionNameSettings != null)
        {
            _captionNameAllowPattern = Pattern.CompilePattern(captionNameSettings.AllowPattern);
            _captionNameDisallowPattern = Pattern.CompilePattern(captionNameSettings.DisallowPattern);
        }

        var fieldNameSettings = LinterSettings.instance?.fieldNamePattern;
        if (fieldNameSettings != null)
        {
            _fieldNameAllowPattern = Pattern.CompilePattern(fieldNameSettings.AllowPattern);
            _fieldNameDisallowPattern = Pattern.CompilePattern(fieldNameSettings.DisallowPattern);
        }
        _apiPageFieldAllowPattern = new Regex(@"^[a-z][A-Za-z0-9]*$"); // See: https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/devenv-api-pagetype#naming-conventions

        var groupNameSettings = LinterSettings.instance?.groupNamePattern;
        if (groupNameSettings != null)
        {
            _groupNameAllowPattern = Pattern.CompilePattern(groupNameSettings.AllowPattern);
            _groupNameDisallowPattern = Pattern.CompilePattern(groupNameSettings.DisallowPattern);
        }

        var actionNameSettings = LinterSettings.instance?.actionNamePattern;
        if (actionNameSettings != null)
        {
            _actionNameAllowPattern = Pattern.CompilePattern(actionNameSettings.AllowPattern);
            _actionNameDisallowPattern = Pattern.CompilePattern(actionNameSettings.DisallowPattern);
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0092NamesPattern);

    public override void Initialize(AnalysisContext context)
    {
        if (_procedureNameAllowPattern != null ||
            _procedureNameDisallowPattern != null ||
            _procedureNameGlobalAllowPattern != null ||
            _procedureNameGlobalDisallowPattern != null ||
            _procedureNameLocalAllowPattern != null ||
            _procedureNameLocalDisallowPattern != null ||
            _eventSubscriberAllowPattern != null ||
            _eventSubscriberDisallowPattern != null ||
            _eventDeclarationAllowPattern != null ||
            _eventDeclarationDisallowPattern != null
           )
        {
            context.RegisterSymbolAction(
                new Action<SymbolAnalysisContext>(this.CheckProcedureName),
                new SymbolKind[]{
                    SymbolKind.Method,
                }
            );
        }

        if (_variableAndParameterNameAllowPattern != null || _variableAndParameterNameDisallowPattern != null)
        {
            context.RegisterSymbolAction(
                new Action<SymbolAnalysisContext>(this.CheckVariableName),
                new SymbolKind[]{
                    SymbolKind.LocalVariable,
                    SymbolKind.GlobalVariable,
                }
            );
            context.RegisterSymbolAction(
                new Action<SymbolAnalysisContext>(this.CheckParameterName),
                new SymbolKind[]{
                    SymbolKind.Method,
                }
            );
        }
        if (_captionNameAllowPattern != null || _captionNameDisallowPattern != null)
        {
            context.RegisterSymbolAction(
                new Action<SymbolAnalysisContext>(this.CheckCaptionName),
                new SymbolKind[]{
                    SymbolKind.Property,
                }
            );
        }
        if (_fieldNameAllowPattern != null || _fieldNameDisallowPattern != null)
        {
            context.RegisterSymbolAction(
                new Action<SymbolAnalysisContext>(this.CheckFieldName),
                new SymbolKind[]{
                    SymbolKind.Field,
                }
            );
        }
        // always register for API page field name pattern, as this is not configurable
        context.RegisterSymbolAction(
            new Action<SymbolAnalysisContext>(this.CheckAPIFieldName),
            new SymbolKind[]{
                SymbolKind.Control,
            }
        );
        if (_groupNameAllowPattern != null || _groupNameDisallowPattern != null ||
            _actionNameAllowPattern != null || _actionNameDisallowPattern != null)
        {
            context.RegisterSymbolAction(
                this.CheckGroupAndActionName,
                new SymbolKind[]{
                    SymbolKind.Page,
                    SymbolKind.PageExtension,
                    SymbolKind.RequestPage,
                }
            );
        }
    }

    private void CheckProcedureName(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IMethodSymbol method)
            return;

        CheckProcedureNameAllowPatterns(ctx, method);
        CheckProcedureNameDisallowPatterns(ctx, method);
    }

    private void CheckProcedureNameDisallowPatterns(SymbolAnalysisContext ctx, IMethodSymbol method)
    {
        var methodName = method.Name;

        if (method.IsEventSubscriber() && _eventSubscriberDisallowPattern != null)
        {
            HelperFunctions.CheckDoesNotMatchPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _eventSubscriberDisallowPattern,
                "event.subscriber.disallow.pattern",
                methodName,
                "Procedure"
            );
        }

        if (method.IsEvent && _eventDeclarationDisallowPattern != null)
        {
            HelperFunctions.CheckDoesNotMatchPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _eventDeclarationDisallowPattern,
                "event.declaration.disallow.pattern",
                methodName,
                "Procedure"
            );
        }

        if (method.IsLocal)
        {
            if (_procedureNameLocalDisallowPattern != null)
            {
                HelperFunctions.CheckDoesNotMatchPattern(
                    ctx,
                    ctx.Symbol.GetLocation(),
                    _procedureNameLocalDisallowPattern,
                    "local.procedure.disallow.pattern",
                    methodName,
                    "Procedure"
                );
            }
        }
        else if (_procedureNameGlobalDisallowPattern != null)
        {
            HelperFunctions.CheckDoesNotMatchPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _procedureNameGlobalDisallowPattern,
                "global.procedure.disallow.pattern",
                methodName,
                "Procedure"
            );
        }

        if (_procedureNameDisallowPattern != null)
        {
            HelperFunctions.CheckDoesNotMatchPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _procedureNameDisallowPattern,
                "disallow.pattern",
                methodName,
                "Procedure"
            );
        }
    }

    private void CheckProcedureNameAllowPatterns(SymbolAnalysisContext ctx, IMethodSymbol method)
    {
        var methodName = method.Name;

        if (method.IsEventSubscriber() && _eventSubscriberAllowPattern != null)
        {
            HelperFunctions.CheckMatchesPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _eventSubscriberAllowPattern,
                "event.subscriber.allow.pattern",
                methodName,
                "Procedure"
            );
        }

        if (method.IsEvent && _eventDeclarationAllowPattern != null)
        {
            HelperFunctions.CheckMatchesPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _eventDeclarationAllowPattern,
                "event.declaration.allow.pattern",
                methodName,
                "Procedure"
            );
        }

        if (method.IsLocal)
        {
            if (_procedureNameLocalAllowPattern != null)
            {
                HelperFunctions.CheckMatchesPattern(
                    ctx,
                    ctx.Symbol.GetLocation(),
                    _procedureNameLocalAllowPattern,
                    "local.procedure.allow.pattern",
                    methodName,
                    "Procedure"
                );
            }
        }
        else if (_procedureNameGlobalAllowPattern != null)
        {
            HelperFunctions.CheckMatchesPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _procedureNameGlobalAllowPattern,
                "global.procedure.allow.pattern",
                methodName,
                "Procedure"
            );
        }

        if (_procedureNameAllowPattern != null)
        {
            HelperFunctions.CheckMatchesPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _procedureNameAllowPattern,
                "allow.pattern",
                methodName,
                "Procedure"
            );
        }
    }

    private void CheckParameterName(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IMethodSymbol methodSym)
            return;

        foreach (var parSym in methodSym.Parameters)
        {
            CheckVariableOrParameterPattern(ctx, parSym.GetLocation(), parSym.Name, "Parameter");
        }

        var retSym = methodSym.ReturnValueSymbol;
        if (retSym != null && retSym.IsNamed)
        {
            CheckVariableOrParameterPattern(ctx, retSym.GetLocation(), retSym.Name, "Return value");
        }
    }

    private void CheckVariableName(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IVariableSymbol varSym)
            return;

        CheckVariableOrParameterPattern(ctx, varSym.GetLocation(), varSym.Name, "Variable");
    }

    private void CheckVariableOrParameterPattern(SymbolAnalysisContext ctx, Location location, string text, string kind)
    {
        if (_variableAndParameterNameAllowPattern != null && !_variableAndParameterNameAllowPattern.IsMatch(text))
        {
            HelperFunctions.CheckMatchesPattern(
                ctx,
                location,
                _variableAndParameterNameAllowPattern,
                "allow.pattern",
                text,
                kind
        );
        }

        if (_variableAndParameterNameDisallowPattern != null && _variableAndParameterNameDisallowPattern.IsMatch(text))
        {
            HelperFunctions.CheckDoesNotMatchPattern(
                    ctx,
                    location,
                    _variableAndParameterNameDisallowPattern,
                    "disallow.pattern",
                    text,
                    kind
            );
        }
    }

    private void CheckCaptionName(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IPropertySymbol caption)
            return;

        if (caption.PropertyKind != PropertyKind.Caption)
            return;


        if (_captionNameAllowPattern != null)
        {
            HelperFunctions.CheckMatchesPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _captionNameAllowPattern,
                "caption.name.allow.pattern",
                caption.ValueText,
                "Caption"
            );
        }
        if (_captionNameDisallowPattern != null)
        {
            HelperFunctions.CheckDoesNotMatchPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _captionNameDisallowPattern,
                "caption.name.disallow.pattern",
                caption.ValueText,
                "Caption"
            );
        }
    }

    private void CheckAPIFieldName(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IControlSymbol field)
            return;

        if (field.ControlKind != ControlKind.Field)
            return;


        if (ctx.Symbol is IControlSymbol ctrlSym && ctrlSym.ControlKind == ControlKind.Field)
        {
            var sym = ctx.Symbol.GetContainingSymbolOfKind<IPageTypeSymbol>(SymbolKind.Page);
            if (sym != null && sym.PageType == PageTypeKind.API)
            {
                // On API Pages, there are specific rules for field names
                HelperFunctions.CheckMatchesPattern(
                    ctx,
                    ctx.Symbol.GetLocation(),
                    _apiPageFieldAllowPattern,
                    "",
                    field.Name,
                    "API-Page field"
                );
            }
            else
            {
                CheckFieldPattern(ctx, ctrlSym.Name, "Page field");
            }
        }
    }

    private void CheckFieldName(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IFieldSymbol field)
            return;

        CheckFieldPattern(ctx, field.Name, "Table field");
    }

    private void CheckFieldPattern(SymbolAnalysisContext ctx, string text, string kind)
    {
        if (_fieldNameAllowPattern != null)
        {
            HelperFunctions.CheckMatchesPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _fieldNameAllowPattern,
                "field.name.allow.pattern",
                text,
                kind
            );
        }

        if (_fieldNameDisallowPattern != null)
        {
            HelperFunctions.CheckDoesNotMatchPattern(
                ctx,
                ctx.Symbol.GetLocation(),
                _fieldNameDisallowPattern,
                "field.name.disallow.pattern",
                text,
                kind
            );
        }
    }

    private void CheckGroupAndActionName(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;


        if (ctx.Symbol is IPageBaseTypeSymbol page)
        {
            foreach (var ctrl in page.FlattenedControls)
            {
                CheckGroupName(ctx, ctrl);
            }

            foreach (var action in page.FlattenedActions)
            {
                CheckActionName(ctx, action);
            }
        }
        else if (ctx.Symbol is IPageExtensionTypeSymbol pageext)
        {
            foreach (var ctrl in pageext.AddedControlsFlattened)
            {
                CheckGroupName(ctx, ctrl);
            }

            foreach (var action in pageext.AddedActionsFlattened)
            {
                CheckActionName(ctx, action);
            }
        }
    }

    private void CheckGroupName(SymbolAnalysisContext ctx, IControlSymbol ctrl)
    {
        if (ctrl.ControlKind == ControlKind.Group && !string.IsNullOrEmpty(ctrl.Name))
        {
            if (_groupNameAllowPattern != null && !_groupNameAllowPattern.IsMatch(ctrl.Name))
            {
                try
                {
                    var identifier = ctrl
                        .DeclaringSyntaxReference?
                        .GetSyntax()
                        .ChildNodes()
                        .First(node => node.IsKind(SyntaxKind.IdentifierName));

                    if (identifier != null)
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.Rule0092NamesPattern,
                            identifier.GetLocation(),
                            [
                                "Group",
                                ctrl.Name,
                                "must",
                                "group.name.allow.pattern",
                                _groupNameAllowPattern,
                            ]
                        ));
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }

            if (_groupNameDisallowPattern != null && _groupNameDisallowPattern.IsMatch(ctrl.Name))
            {
                try
                {
                    var identifier = ctrl
                        .DeclaringSyntaxReference?
                        .GetSyntax()
                        .ChildNodes()
                        .First(node => node.IsKind(SyntaxKind.IdentifierName));

                    if (identifier != null)
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.Rule0092NamesPattern,
                            identifier.GetLocation(),
                            [
                                "Group",
                                ctrl.Name,
                                "must not",
                                "group.name.disallow.pattern",
                                _groupNameDisallowPattern,
                            ]
                        ));
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
    
    private void CheckActionName(SymbolAnalysisContext ctx, IActionSymbol action)
    {
        if (!string.IsNullOrEmpty(action.Name))
        {
            if (_actionNameAllowPattern != null && !_actionNameAllowPattern.IsMatch(action.Name))
            {
                try
                {
                    var identifier = action
                        .DeclaringSyntaxReference?
                        .GetSyntax()
                        .ChildNodes()
                        .First(node => node.IsKind(SyntaxKind.IdentifierName));

                    if (identifier != null)
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.Rule0092NamesPattern,
                            identifier.GetLocation(),
                            [
                                "Action",
                                action.Name,
                                "must",
                                "action.name.allow.pattern",
                                _actionNameAllowPattern,
                            ]
                        ));
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }

            if (_actionNameDisallowPattern != null && _actionNameDisallowPattern.IsMatch(action.Name))
            {
                try
                {
                    var identifier = action
                        .DeclaringSyntaxReference?
                        .GetSyntax()
                        .ChildNodes()
                        .First(node => node.IsKind(SyntaxKind.IdentifierName));

                    if (identifier != null)
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.Rule0092NamesPattern,
                            identifier.GetLocation(),
                            [
                                "Action",
                                action.Name,
                                "must not",
                                "action.name.disallow.pattern",
                                _actionNameDisallowPattern,
                            ]
                        ));
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}