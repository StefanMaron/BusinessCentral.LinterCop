using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0012DoNotUseObjectIdInSystemFunctions : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0012DoNotUseObjectIdInSystemFunctions);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(new Action<OperationAnalysisContext>(this.CheckForObjectIdsInFunctionInvocations), OperationKind.InvocationExpression);
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckForObjectIdsEventSubscribers), SymbolKind.Method);
        }
        private void CheckForObjectIdsEventSubscribers(SymbolAnalysisContext context)
        {
            IMethodSymbol method = (IMethodSymbol)context.Symbol;
            if (method.Attributes.Length == 0)
                return;

            var ObjectAccessToUse = method.Attributes[0].DeclaringSyntaxReference.GetSyntax().DescendantNodes(o => true).FirstOrDefault(n => n.IsKind(SyntaxKind.OptionAccessExpression));
            if (ObjectAccessToUse == null)
                return;

            var ObjectAccessToUseText = ObjectAccessToUse.DescendantNodes().ToArray()[1].ToString();
            if (ObjectAccessToUseText == "Table")
                ObjectAccessToUseText = "Database";

            var wrongSyntaxLiteral = method.Attributes[0].DeclaringSyntaxReference.GetSyntax().DescendantNodes(o => true).FirstOrDefault(n => n.IsKind(SyntaxKind.Int32SignedLiteralValue));

            if (wrongSyntaxLiteral != null)
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0012DoNotUseObjectIdInSystemFunctions, wrongSyntaxLiteral.GetLocation(), new object[] { ObjectAccessToUseText, "" }));
        }

        private void CheckForObjectIdsInFunctionInvocations(OperationAnalysisContext context)
        {
            if (context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || context.ContainingSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (context.ContainingSymbol.IsObsoletePending || context.ContainingSymbol.IsObsoleteRemoved) return;
            IInvocationExpression operation = (IInvocationExpression)context.Operation;
            RelevantFuntion CurrentFunction = null;
            try
            {
                CurrentFunction = FunctionCallsWithIDParamaters.RelevantFunctions.First(o => (o.ObjectType.ToString().ToUpper() == operation.TargetMethod.ContainingSymbol.Name.ToUpper() && o.FunctionName == operation.TargetMethod.Name));
            }
            catch (System.InvalidOperationException)
            { }

            SyntaxKind[] AllowedParameterKinds = { SyntaxKind.MemberAccessExpression, SyntaxKind.IdentifierName, SyntaxKind.InvocationExpression };

            if (CurrentFunction != null && operation.TargetMethod.Parameters.Length != 0 && !AllowedParameterKinds.Contains(operation.Arguments[0].Syntax.Kind) && (operation.Arguments[0].Syntax.ToString() != "0" || !CurrentFunction.ZeroIDAllowed))
            {
                if (operation.TargetMethod.Parameters[0].ParameterType.NavTypeKind == NavTypeKind.Integer)
                {
                    int tempint = 0;
                    if (int.TryParse(operation.Arguments[0].Syntax.ToString(), out tempint))
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0012DoNotUseObjectIdInSystemFunctions, context.Operation.Syntax.GetLocation(), new object[] { CurrentFunction.CorrectAccessSymbol, "" }));
                    else
                        if (!operation.Arguments[0].Syntax.ToString().ToUpper().StartsWith(CurrentFunction.CorrectAccessSymbol.ToUpper()))
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0012DoNotUseObjectIdInSystemFunctions, context.Operation.Syntax.GetLocation(), new object[] { CurrentFunction.CorrectAccessSymbol, "" }));
                }
            }

        }
    }

    class RelevantFuntion
    {
        public NavTypeKind ObjectType;
        public string FunctionName;
        public string CorrectAccessSymbol;
        public bool ZeroIDAllowed;

        public RelevantFuntion(NavTypeKind ObjType, string FnctName, string AccessSymbol, bool ZeroID)
        {
            ObjectType = ObjType;
            FunctionName = FnctName;
            CorrectAccessSymbol = AccessSymbol;
            ZeroIDAllowed = ZeroID;
        }
    }

    class FunctionCallsWithIDParamaters
    {
        static public List<RelevantFuntion> RelevantFunctions = new List<RelevantFuntion> {
            new RelevantFuntion(NavTypeKind.Codeunit,"Run","Codeunit",false),
            new RelevantFuntion(NavTypeKind.Report,"Run","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"Execute","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"Print","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"RdlcLayout","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"RunModal","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"RunRequestPage","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"SaveAs","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"SaveAsExcel","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"SaveAsHtml","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"SaveAsPdf","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"SaveAsWord","Report",false),
            new RelevantFuntion(NavTypeKind.Report,"SaveAsXml","Report",false),
            new RelevantFuntion(NavTypeKind.XmlPort,"Run","XMLPort",false),
            new RelevantFuntion(NavTypeKind.XmlPort,"Export","XMLPort",false),
            new RelevantFuntion(NavTypeKind.XmlPort,"Import","XMLPort",false),
            new RelevantFuntion(NavTypeKind.Page,"Run","Page",true),
            new RelevantFuntion(NavTypeKind.Page,"RunModal","Page",true),
            new RelevantFuntion(NavTypeKind.RecordRef,"Open","Database",false),
            new RelevantFuntion(NavTypeKind.Query,"SaveAsCsv","Query",false),
            new RelevantFuntion(NavTypeKind.Query,"SaveAsXml","Query",false)
        };
    }
}
