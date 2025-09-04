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
}