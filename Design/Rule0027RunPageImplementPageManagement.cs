using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0027RunPageImplementPageManagement : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0027RunPageImplementPageManagement);

        public override void Initialize(AnalysisContext context) => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckRunPageImplementPageManagement), OperationKind.InvocationExpression);

        private void CheckRunPageImplementPageManagement(OperationAnalysisContext ctx)
        {
            if (ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || ctx.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (ctx.ContainingSymbol.IsObsoletePending || ctx.ContainingSymbol.IsObsoleteRemoved) return;

            IInvocationExpression operation = (IInvocationExpression)ctx.Operation;
            if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod) return;

            if (operation.TargetMethod.ContainingType.GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Page) return;
            if (operation.Arguments.Count() < 2) return;

            // do not execute on CurrPage.EnqueueBackgroundTask
            if (SemanticFacts.IsSameName(operation.TargetMethod.Name, "EnqueueBackgroundTask")) return;

            // Page Management Codeunit doesn't support returntype Action
            if (operation.TargetMethod.ReturnValueSymbol.ReturnType.GetNavTypeKindSafe() == NavTypeKind.Action) return;

            // In case the PageID is set by a field from a (setup) record or a method
            if (!operation.Arguments[0].Syntax.IsKind(SyntaxKind.OptionAccessExpression)) return;

            if (IsSupportedRecord(((IConversionExpression)operation.Arguments[1].Value).Operand))
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0027RunPageImplementPageManagement, ctx.Operation.Syntax.GetLocation()));
        }

        private static bool IsSupportedRecord(IOperation operation)
        {
            IRecordTypeSymbol recordTypeSymbol = null;

            if (operation.Kind == OperationKind.GlobalReferenceExpression || operation.Kind == OperationKind.LocalReferenceExpression)
                recordTypeSymbol = (IRecordTypeSymbol)operation.GetSymbol().GetTypeSymbol();

            if (operation.Kind == OperationKind.InvocationExpression)
                recordTypeSymbol = (IRecordTypeSymbol)operation.Type.GetTypeSymbol();

            if (recordTypeSymbol == null || recordTypeSymbol.Temporary) return false;

            if (GetSupportedRecords().ContainsKey(recordTypeSymbol.Id))
                return SemanticFacts.IsSameName(recordTypeSymbol.Name, GetSupportedRecords()[recordTypeSymbol.Id]);

            return false;
        }

        private static Dictionary<int, string> GetSupportedRecords()
        {
            Dictionary<int, string> SupportedRecords = new Dictionary<int, string>
            {
                { 36, "Sales Header" },
                { 38, "Purchase Header" },
                { 79, "Company Information" },
                { 80, "Gen. Journal Template" },
                { 81, "Gen. Journal Line" },
                { 91, "User Setup" },
                { 98, "General Ledger Setup" },
                { 112, "Sales Invoice Header" },
                { 131, "Incoming Documents Setup" },
                { 207, "Res. Journal Line" },
                { 210, "Job Journal Line" },
                { 232, "Gen. Journal Batch" },
                { 312, "Purchases & Payables Setup" },
                { 454, "Approval Entry" },
                { 843, "Cash Flow Setup" },
                { 1251, "Text-to-Account Mapping" },
                { 1275, "Doc. Exch. Service Setup" },
                { 5107, "Sales Header Archive" },
                { 5109, "Purchase Header Archive" },
                { 5200, "Employee" },
                { 5405, "Production Order" },
                { 5900, "Service Header" },
                { 5965, "Service Contract Header" },
                { 7152, "Item Analysis View" },
                { 2000000120, "User" }
            };
            return SupportedRecords;
        }
    }
}