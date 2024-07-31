using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0068EmptyStatements : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0068EmptyStatements);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeEmptyStatement), OperationKind.EmptyStatement);

        private void AnalyzeEmptyStatement(OperationAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            // exclude empty ifs (runtime error guard) and empty case lines
            if (ctx.Operation.Syntax.Parent.IsKind(SyntaxKind.IfStatement) || ctx.Operation.Syntax.Parent.IsKind(SyntaxKind.CaseLine)) return;

            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0068EmptyStatements, ctx.Operation.Syntax.GetLocation()));
        }
    }
}