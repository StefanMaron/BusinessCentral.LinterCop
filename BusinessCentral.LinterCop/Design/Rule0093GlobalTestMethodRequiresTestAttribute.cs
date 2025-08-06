using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0093GlobalTestMethodRequiresTestAttribute : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DiagnosticDescriptors.Rule0093GlobalTestMethodRequiresTestAttribute
        );

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
        if (ctx.Symbol is IMethodSymbol method && !method.IsLocal)
        {
            if (IsTestCodeunit(ctx.Symbol.GetContainingSymbolOfKind<IObjectTypeSymbol>(SymbolKind.Codeunit)))
            {
                if (!IsTestMethod(method) && !IsHandlerMethod(method))
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.Rule0093GlobalTestMethodRequiresTestAttribute,
                        ctx.Symbol.GetLocation(),
                        [method.Name]
                    ));
                }
            }
        }
    }

    private bool IsTestCodeunit(IObjectTypeSymbol? symbol)
    {
        if (symbol == null)
        {
            return false;
        }

        var property = symbol.GetEnumPropertyValue<CodeunitSubtypeKind>(PropertyKind.Subtype);
        return property != null && property == CodeunitSubtypeKind.Test;
    }

    private bool IsTestMethod(IMethodSymbol method) =>
        method.Attributes.Any(attr => attr.AttributeKind == AttributeKind.Test);

    private bool IsHandlerMethod(IMethodSymbol method) =>
        method.Attributes.Any(attr => IsHandlerAttribute(attr.AttributeKind));

    private bool IsHandlerAttribute(AttributeKind kind)
    {
        switch (kind)
        {
            case AttributeKind.ConfirmHandler:
            case AttributeKind.FilterPageHandler:
            case AttributeKind.HyperlinkHandler:
            case AttributeKind.MessageHandler:
            case AttributeKind.ModalPageHandler:
            case AttributeKind.PageHandler:
            case AttributeKind.RecallNotificationHandler:
            case AttributeKind.ReportHandler:
            case AttributeKind.RequestPageHandler:
            case AttributeKind.SendNotificationHandler:
            case AttributeKind.SessionSettingsHandler:
            case AttributeKind.StrMenuHandler:
                return true;
        }

        return false;
    }
}