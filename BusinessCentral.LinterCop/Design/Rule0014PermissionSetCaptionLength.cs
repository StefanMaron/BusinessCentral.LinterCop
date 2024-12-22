using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0014PermissionSetCaptionLength : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0014PermissionSetCaptionLength);

    private const int MAXCAPTIONLENGTH = 30;

    public override void Initialize(AnalysisContext context)
        => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckPermissionSetNameAndCaptionLength), SymbolKind.PermissionSet);

    private void CheckPermissionSetNameAndCaptionLength(SymbolAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        IPropertySymbol? captionProperty = ctx.Symbol.GetProperty(PropertyKind.Caption);
        if (captionProperty is null)
            return;

        if (captionProperty.ValueText.Length > MAXCAPTIONLENGTH)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0014PermissionSetCaptionLength,
                captionProperty.GetLocation(),
                MAXCAPTIONLENGTH));

        var subProperties = ExtractSubProperties(captionProperty);
        if (subProperties is null || subProperties.Any(node => node.ToString().Contains("Locked", StringComparison.OrdinalIgnoreCase)))
            return;

        var maxLengthNode = subProperties.FirstOrDefault(node => node.ToString().Contains("MaxLength", StringComparison.OrdinalIgnoreCase));
        if (maxLengthNode is not null &&
            int.TryParse(maxLengthNode.DescendantNodes().FirstOrDefault(e => e.Kind == SyntaxKind.Int32SignedLiteralValue)?.ToString(), out int maxLength))
        {
            if (maxLength > MAXCAPTIONLENGTH)
            {
                if (captionProperty?.ValueText.Length > MAXCAPTIONLENGTH)
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.Rule0014PermissionSetCaptionLength,
                        captionProperty.GetLocation(),
                        MAXCAPTIONLENGTH));

            }
            return;
        }

        if (captionProperty is not null)
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0014PermissionSetCaptionLength,
                captionProperty.GetLocation(),
                MAXCAPTIONLENGTH));
    }

    private IEnumerable<SyntaxNode> ExtractSubProperties(IPropertySymbol? captionProperty)
    {
        var syntaxReference = captionProperty?.DeclaringSyntaxReference;
        if (syntaxReference is null)
            return Enumerable.Empty<SyntaxNode>();

        var syntaxNode = syntaxReference.GetSyntax();
        if (syntaxNode is null)
            return Enumerable.Empty<SyntaxNode>();

        var subPropertyNode = syntaxNode.DescendantNodes()
            .FirstOrDefault(e => e.Kind == SyntaxKind.CommaSeparatedIdentifierEqualsLiteralList);

        return subPropertyNode?.DescendantNodes() ?? Enumerable.Empty<SyntaxNode>();
    }
}