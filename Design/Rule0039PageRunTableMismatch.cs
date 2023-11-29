using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0039PageRunTableMismatch : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0039ArgumentDifferentTypeThenExpected);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.AnalyzeRunPageArguments), OperationKind.InvocationExpression);

        private void AnalyzeRunPageArguments(OperationAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            if (operation.TargetMethod.ContainingType.GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Page) return;
            if (operation.Arguments.Count() < 2) return;

            if (operation.Arguments[0].Syntax.Kind != SyntaxKind.OptionAccessExpression) return;
            if (operation.Arguments[1].Syntax.Kind != SyntaxKind.IdentifierName) return;

            IApplicationObjectTypeSymbol applicationObjectTypeSymbol = ((IApplicationObjectAccess)operation.Arguments[0].Value).ApplicationObjectTypeSymbol;
            if (applicationObjectTypeSymbol.GetNavTypeKindSafe() != NavTypeKind.Page) return;
            ITableTypeSymbol pageObjSourceTable = ((IPageTypeSymbol)applicationObjectTypeSymbol.GetTypeSymbol()).RelatedTable;
            if (pageObjSourceTable == null) return;

            IOperation operand = ((IConversionExpression)operation.Arguments[1].Value).Operand;
            ITableTypeSymbol recordArgument = ((IRecordTypeSymbol)operand.GetSymbol().GetTypeSymbol()).BaseTable;

            if (recordArgument != pageObjSourceTable)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0039ArgumentDifferentTypeThenExpected, ctx.Operation.Syntax.GetLocation(), new object[] { 2, operand.GetSymbol().GetTypeSymbol().ToString(), "Record \"" + pageObjSourceTable.Name + "\"" }));
        }
    }
}