using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0005CasingMismatchKeyword : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create(DiagnosticDescriptors.Rule0005CasingMismatch);

    private static readonly Lazy<HashSet<SyntaxKind>> _dataTypeSyntaxKinds =
        new Lazy<HashSet<SyntaxKind>>(() =>
            Enum.GetNames(typeof(SyntaxKind))
                .Where(name => name.AsSpan().EndsWith("DataType"))
                .Select(Enum.Parse<SyntaxKind>)
                .ToHashSet());

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeTokens), new SymbolKind[] {
                SymbolKind.Codeunit,
                SymbolKind.Entitlement,
                SymbolKind.Enum,
                SymbolKind.EnumExtension,
                SymbolKind.Interface,
                SymbolKind.Page,
                SymbolKind.PageExtension,
                SymbolKind.PermissionSet,
                SymbolKind.PermissionSetExtension,
                SymbolKind.Profile,
                SymbolKind.ProfileExtension,
                SymbolKind.Query,
                SymbolKind.Report,
                SymbolKind.ReportExtension,
                SymbolKind.Table,
                SymbolKind.TableExtension,
                SymbolKind.XmlPort
            });
    }

    private void AnalyzeTokens(SymbolAnalysisContext ctx)
    {
        var node = ctx.Symbol.DeclaringSyntaxReference?.GetSyntax(ctx.CancellationToken);
        if (node is null)
            return;

        var dataTypeSyntaxKinds = _dataTypeSyntaxKinds.Value;

        foreach (SyntaxToken token in node.DescendantTokens())
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            string? tokenText = token.ValueText;
            if (string.IsNullOrEmpty(tokenText))
                continue;

            if (token.Parent is null || !token.Kind.IsKeyword() || dataTypeSyntaxKinds.Contains(token.Parent.Kind))
                continue;

            SyntaxToken canonicalToken = SyntaxFactory.Token(token.Kind);
            if (canonicalToken.Kind == SyntaxKind.None)
                continue;

            if (!canonicalToken.ValueText.AsSpan().Equals(tokenText.AsSpan(), StringComparison.Ordinal))
            {
                ctx.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.Rule0005CasingMismatch,
                        token.GetLocation(),
                        canonicalToken));
            }
        }
    }
}