using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using BusinessCentral.LinterCop.Helpers;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0009CodeMetrics : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0009CodeMetricsInfo, DiagnosticDescriptors.Rule0010CodeMetricsWarning);

        public override void Initialize(AnalysisContext context)
            => context.RegisterCodeBlockAction(new Action<CodeBlockAnalysisContext>(this.CheckCodeMetrics));

        private void CheckCodeMetrics(CodeBlockAnalysisContext context)
        {
            if (context.OwningSymbol.GetContainingObjectTypeSymbol().IsObsoletePending || context.OwningSymbol.GetContainingObjectTypeSymbol().IsObsoleteRemoved) return;
            if (context.OwningSymbol.IsObsoletePending ||context.OwningSymbol.IsObsoleteRemoved) return;
            if (context.OwningSymbol.GetContainingObjectTypeSymbol().NavTypeKind == NavTypeKind.Interface || context.OwningSymbol.GetContainingObjectTypeSymbol().NavTypeKind == NavTypeKind.ControlAddIn) return;

            int cyclomaticComplexity = GetCyclomaticComplexity(context.CodeBlock);
            double HalsteadVolume = GetHalsteadVolume(context.CodeBlock, ref context, cyclomaticComplexity);

            LinterSettings.Create();
            if (LinterSettings.instance != null)
                if (cyclomaticComplexity >= LinterSettings.instance.cyclomaticComplexityThreshold || Math.Round(HalsteadVolume) <= LinterSettings.instance.maintainabilityIndexThreshold)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0010CodeMetricsWarning, context.OwningSymbol.GetLocation(), new object[] { cyclomaticComplexity, Math.Round(HalsteadVolume) }));
                    return;
                }

            if (cyclomaticComplexity >= LinterSettings.instance.cyclomaticComplexityThreshold || Math.Round(HalsteadVolume) <= LinterSettings.instance.maintainabilityIndexThreshold)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0010CodeMetricsWarning, context.OwningSymbol.GetLocation(), new object[] { LinterSettings.instance.cyclomaticComplexityThreshold, Math.Round(HalsteadVolume) }));
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0009CodeMetricsInfo, context.OwningSymbol.GetLocation(), new object[] { cyclomaticComplexity, Math.Round(HalsteadVolume)}));
        }

        private static double GetHalsteadVolume(SyntaxNode CodeBlock, ref CodeBlockAnalysisContext context,int cyclomaticComplexity)
        {
            try
            {
                var Syntax = CodeBlock.DescendantNodesAndTokens(e => true).ToList();
                var methodBody = Syntax.Find(node => node.Kind == SyntaxKind.Block && (node.Parent.Kind == SyntaxKind.MethodDeclaration || node.Parent.Kind == SyntaxKind.TriggerDeclaration));

                var OperandKinds = new object[] { SyntaxKind.IdentifierToken, SyntaxKind.Int32LiteralToken, SyntaxKind.StringLiteralToken, SyntaxKind.BooleanLiteralValue, SyntaxKind.TrueKeyword, SyntaxKind.FalseKeyword };
                var OperatorKinds = Enum.GetValues(typeof(SyntaxKind)).Cast<SyntaxKind>().Where(value => (value.ToString().Contains("Keyword") || value.ToString().Contains("Token")) && !OperandKinds.Contains(value)).ToList();
                var TriviaKinds = Enum.GetValues(typeof(SyntaxKind)).Cast<SyntaxKind>().Where(value => (value.ToString().Contains("Trivia"))).ToList();

                var triviaLines = methodBody.AsNode().DescendantTrivia(e => true, true).Where(node => node.Kind == SyntaxKind.EndOfLineTrivia);
                triviaLines = triviaLines.Where(node => node.GetLocation().GetLineSpan().StartLinePosition.Line == node.Token.GetLocation().GetLineSpan().StartLinePosition.Line);
                var triviaLinesCount = triviaLines.Count() - 2;//Minus 2 for Begin end of function


                var Operands = methodBody.AsNode().DescendantNodesAndTokens(e => true).Where(node => OperandKinds.Contains(node.Kind));
                var Operators = methodBody.AsNode().DescendantNodesAndTokens(e => true).Where(node => OperatorKinds.Contains(node.Kind));

                double N = (double)(Operators.Count() + Operands.Count());
                double n = (double)Operators.Distinct().Count() + Operands.Distinct().Count();
                double HalsteadVolume = N * Math.Log(n, 2);

                //171−5.2lnV−0.23G−16.2lnL
                return Math.Max(0, (171 - 5.2 * Math.Log(HalsteadVolume) - 0.23 * cyclomaticComplexity - 16.2 * Math.Log(triviaLinesCount)) * 100 / 171);

            }
            catch (System.NullReferenceException)
            {
                return 0.0;
            }
        }
        private static int GetCyclomaticComplexity(SyntaxNode CodeBlock)
        {
            var Syntax = CodeBlock.DescendantNodesAndTokens(e => true).ToList();
            var ComplexKinds = new object[] { SyntaxKind.IfKeyword, SyntaxKind.ElifKeyword, SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression, SyntaxKind.CaseLine, SyntaxKind.ForKeyword, SyntaxKind.ForEachKeyword, SyntaxKind.WhileKeyword, SyntaxKind.UntilKeyword };
            var nodes = Syntax.Count(node =>
            {
                if (node.IsNode)
                {
                    return ComplexKinds.Contains(
                    node.AsNode().Kind);
                }
                if (node.IsToken)
                {
                    return ComplexKinds.Contains(
                    node.AsToken().Kind);
                }
                else return false;
            });
            return nodes + 1;
        }
    }
}


