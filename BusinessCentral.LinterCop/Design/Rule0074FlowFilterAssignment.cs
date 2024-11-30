using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0074FlowFilterAssignment : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0074FlowFilterAssignment);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeAssignmentStatement), new SyntaxKind[] {
            SyntaxKind.AssignmentStatement,
            SyntaxKind.CompoundAssignmentStatement
            });
    }

    private void AnalyzeAssignmentStatement(SyntaxNodeAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved())
            return;

        var target = ctx.Node switch
        {
            AssignmentStatementSyntax assignment => assignment.Target,
            CompoundAssignmentStatementSyntax compoundAssignment => compoundAssignment.Target,
            _ => null
        };

        if (target == null || target.Kind != SyntaxKind.MemberAccessExpression)
            return;

        if (ctx.SemanticModel.GetSymbolInfo(target, ctx.CancellationToken).Symbol is not IFieldSymbol fieldSymbol)
            return;

        if (fieldSymbol.FieldClass == FieldClassKind.FlowFilter)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0074FlowFilterAssignment,
                target.GetIdentifierNameSyntax().GetLocation(), new object[] { fieldSymbol.Name.ToString().QuoteIdentifierIfNeeded() }));
        }
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0074FlowFilterAssignment = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0074",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0074FlowFilterAssignmentTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0074FlowFilterAssignmentFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0074FlowFilterAssignmentDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0074");
    }
}