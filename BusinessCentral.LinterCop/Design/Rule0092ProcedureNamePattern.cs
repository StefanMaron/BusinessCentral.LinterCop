using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0092ProcedureNamePattern : DiagnosticAnalyzer
{
    private Regex? _allowPattern = null;
    private Regex? _disallowPattern = null;
    private Regex? _globalAllowPattern = null;
    private Regex? _globalDisallowPattern = null;
    private Regex? _localAllowPattern = null;
    private Regex? _localDisallowPattern = null;
    private Regex? _eventSubscriberAllowPattern = null;
    private Regex? _eventSubscriberDisallowPattern = null;
    private Regex? _eventDeclarationAllowPattern = null;
    private Regex? _eventDeclarationDisallowPattern = null;

    public Rule0092ProcedureNamePattern()
    {
        var settings = LinterSettings.instance?.procedureNamePattern;
        if (settings == null)
            return;

        _allowPattern = Pattern.CompilePattern(settings.AllowPattern);
        _disallowPattern = Pattern.CompilePattern(settings.DisallowPattern);
        _globalAllowPattern = Pattern.CompilePattern(settings.GlobalProcedureAllowPattern);
        _globalDisallowPattern = Pattern.CompilePattern(settings.GlobalProcedureDisallowPattern);
        _localAllowPattern = Pattern.CompilePattern(settings.LocalProcedureAllowPattern);
        _localDisallowPattern = Pattern.CompilePattern(settings.LocalProcedureDisallowPattern);
        _eventSubscriberAllowPattern = Pattern.CompilePattern(settings.EventSubscriberAllowPattern);
        _eventSubscriberDisallowPattern = Pattern.CompilePattern(settings.EventSubscriberDisallowPattern);
        _eventDeclarationAllowPattern = Pattern.CompilePattern(settings.EventDeclarationAllowPattern);
        _eventDeclarationDisallowPattern = Pattern.CompilePattern(settings.EventDeclarationDisallowPattern);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0092ProcedureNamePattern);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSymbolAction(
            new Action<SymbolAnalysisContext>(this.Check),
            new SymbolKind[]{
                    SymbolKind.Method,
            }
        );
    }

    private void Check(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Symbol is not IMethodSymbol method)
            return;

        CheckAllowPatterns(ctx, method);
        CheckDisallowPatterns(ctx, method);
    }

    private void CheckDisallowPatterns(SymbolAnalysisContext ctx, IMethodSymbol method)
    {
        var methodName = method.Name;

        if (method.IsEventSubscriber() && _eventSubscriberDisallowPattern != null)
        {
            CheckDoesNotMatchPattern(
                ctx,
                _eventSubscriberDisallowPattern,
                "event.subscriber.disallow.pattern",
                methodName
            );
        }

        if (method.IsEvent && _eventDeclarationDisallowPattern != null)
        {
            CheckDoesNotMatchPattern(
                ctx,
                _eventDeclarationDisallowPattern,
                "event.declaration.disallow.pattern",
                methodName
            );
        }

        if (method.IsLocal)
        {
            if (_localDisallowPattern != null)
            {
                CheckDoesNotMatchPattern(
                    ctx,
                    _localDisallowPattern,
                    "local.procedure.disallow.pattern",
                    methodName
                );
            }
        }
        else if (_globalDisallowPattern != null)
        {
            CheckDoesNotMatchPattern(
                ctx,
                _globalDisallowPattern,
                "global.procedure.disallow.pattern",
                methodName
            );
        }

        if (_disallowPattern != null)
        {
            CheckDoesNotMatchPattern(
                ctx,
                _disallowPattern,
                "disallow.pattern",
                methodName
            );
        }
    }

    private void CheckAllowPatterns(SymbolAnalysisContext ctx, IMethodSymbol method)
    {
        var methodName = method.Name;

        if (method.IsEventSubscriber() && _eventSubscriberAllowPattern != null)
        {
            CheckMatchesPattern(
                ctx,
                _eventSubscriberAllowPattern,
                "event.subscriber.allow.pattern",
                methodName
            );
        }

        if (method.IsEvent && _eventDeclarationAllowPattern != null)
        {
            CheckMatchesPattern(
                ctx,
                _eventDeclarationAllowPattern,
                "event.declaration.allow.pattern",
                methodName
            );
        }

        if (method.IsLocal)
        {
            if (_localAllowPattern != null)
            {
                CheckMatchesPattern(
                    ctx,
                    _localAllowPattern,
                    "local.procedure.allow.pattern",
                    methodName
                );
            }
        }
        else if (_globalAllowPattern != null)
        {
            CheckMatchesPattern(
                ctx,
                _globalAllowPattern,
                "global.procedure.allow.pattern",
                methodName
            );
        }

        if (_allowPattern != null)
        {
            CheckMatchesPattern(
                ctx,
                _allowPattern,
                "allow.pattern",
                methodName
            );
        }
    }

    private void CheckMatchesPattern(SymbolAnalysisContext ctx, Regex pattern, string patternSource, string name)
    {
        CheckPattern(ctx, pattern, patternSource, true, name);
    }

    private void CheckDoesNotMatchPattern(SymbolAnalysisContext ctx, Regex pattern, string patternSource, string name)
    {
        CheckPattern(ctx, pattern, patternSource, false, name);
    }

    private void CheckPattern(SymbolAnalysisContext ctx, Regex pattern, string patternSource, bool isMatch, string name)
    {
        bool matches;
        try
        {
            matches = pattern.IsMatch(name);
        }
        catch (RegexMatchTimeoutException)
        {
            return;
        }
        catch (Exception)
        {
            return;
        }

        if (matches != isMatch)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0092ProcedureNamePattern,
                ctx.Symbol.GetLocation(),
                [
                    name,
                    isMatch ? "must" : "must not",
                    patternSource,
                    pattern.ToString(),
                ]
            ));
        }
    }
}