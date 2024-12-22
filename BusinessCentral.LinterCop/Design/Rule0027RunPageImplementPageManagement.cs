using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0027RunPageImplementPageManagement : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.Rule0027RunPageImplementPageManagement);

    private static readonly Dictionary<int, string> supportedRecords = new Dictionary<int, string>
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

    public override void Initialize(AnalysisContext context)
        => context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckRunPageImplementPageManagement), OperationKind.InvocationExpression);

    private void CheckRunPageImplementPageManagement(OperationAnalysisContext ctx)
    {
        if (ctx.IsObsoletePendingOrRemoved() || ctx.Operation is not IInvocationExpression operation)
            return;

        if (operation.TargetMethod.MethodKind != MethodKind.BuiltInMethod ||
            operation.TargetMethod.Name != "EnqueueBackgroundTask" ||                                           // do not execute on CurrPage.EnqueueBackgroundTask
            operation.TargetMethod.ContainingType?.GetTypeSymbol().GetNavTypeKindSafe() != NavTypeKind.Page ||
            operation.TargetMethod.ReturnValueSymbol.ReturnType.GetNavTypeKindSafe() == NavTypeKind.Action ||   // Page Management Codeunit doesn't support returntype Action
            operation.Arguments.Length < 2)
            return;

        switch (operation.Arguments[0].Syntax.Kind)
        {
            case SyntaxKind.LiteralExpression:
                if (operation.Arguments[0].Syntax.GetIdentifierOrLiteralValue() == "0")
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.Rule0027RunPageImplementPageManagement,
                        ctx.Operation.Syntax.GetLocation()));
                break;

            case SyntaxKind.OptionAccessExpression:
                if (IsSupportedRecord(((IConversionExpression)operation.Arguments[1].Value).Operand))
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.Rule0027RunPageImplementPageManagement,
                        ctx.Operation.Syntax.GetLocation()));
                break;
        }
    }

    private static bool IsSupportedRecord(IOperation operation)
    {
        IRecordTypeSymbol? recordTypeSymbol = null;
        switch (operation.Kind)
        {
            case OperationKind.GlobalReferenceExpression:
            case OperationKind.LocalReferenceExpression:
                recordTypeSymbol = operation.GetSymbol()?.GetTypeSymbol() as IRecordTypeSymbol;
                break;
            case OperationKind.InvocationExpression:
                recordTypeSymbol = operation.Type.GetTypeSymbol() as IRecordTypeSymbol;
                break;
            default:
                return false;
        }

        if (recordTypeSymbol is null || recordTypeSymbol.Temporary)
            return false;

        if (supportedRecords.ContainsKey(recordTypeSymbol.Id))
            return SemanticFacts.IsSameName(recordTypeSymbol.Name, supportedRecords[recordTypeSymbol.Id]);

        return false;
    }
}