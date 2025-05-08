using System.Collections.Immutable;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0074FlowFilterAssignment : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.Rule0074FlowFilterAssignment);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeAssignmentStatement,
            SyntaxKind.AssignmentStatement,
            SyntaxKind.CompoundAssignmentStatement);
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

        if (target is not { Kind: SyntaxKind.MemberAccessExpression })
            return;

        if (ctx.SemanticModel.GetSymbolInfo(target, ctx.CancellationToken).Symbol is not IFieldSymbol fieldSymbol)
            return;

        if (fieldSymbol.FieldClass == FieldClassKind.FlowFilter)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.Rule0074FlowFilterAssignment,
                target.GetIdentifierNameSyntax().GetLocation(),
                fieldSymbol.Name.QuoteIdentifierIfNeeded()));
        }
    }
}