#nullable disable // TODO: Enable nullable and review rule
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0028CodeNavigabilityOnEventSubscribers : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0028IdentifiersInEventSubscribers);

    public override void Initialize(AnalysisContext context) =>
        context.RegisterCodeBlockAction(new Action<CodeBlockAnalysisContext>(this.AnalyzeIdentifiersInEventSubscribers));

    private void AnalyzeIdentifiersInEventSubscribers(CodeBlockAnalysisContext ctx)
    {
        if (!VersionChecker.IsSupported(ctx.OwningSymbol, Feature.IdentifiersInEventSubscribers))
            return;

        if (ctx.IsObsoletePendingOrRemoved() || !ctx.CodeBlock.IsKind(SyntaxKind.MethodDeclaration))
            return;

        if (ctx.CodeBlock is not MethodDeclarationSyntax syntax)
            return;

        var syntaxList = syntax.Attributes.Where(value => SemanticFacts.IsSameName(value.GetIdentifierOrLiteralValue(), "EventSubscriber"));

        var eventName = syntaxList.Where(value => value.ArgumentList.Arguments[2].IsKind(SyntaxKind.LiteralAttributeArgument)).FirstOrDefault();
        if (eventName is not null)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0028IdentifiersInEventSubscribers,
                eventName.GetLocation()));
            return;
        }

        var elementName = syntaxList.Where(value => !(value.ArgumentList.Arguments[3].GetIdentifierOrLiteralValue() == "") && value.ArgumentList.Arguments[3].IsKind(SyntaxKind.LiteralAttributeArgument)).FirstOrDefault();
        if (elementName is not null)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0028IdentifiersInEventSubscribers,
                elementName.GetLocation()));
    }
}