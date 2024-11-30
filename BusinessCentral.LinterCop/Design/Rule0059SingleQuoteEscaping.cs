using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0059SingleQuoteEscaping : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0050OperatorAndPlaceholderInFilterExpression, DiagnosticDescriptors.Rule0059SingleQuoteEscapingIssueDetected);

        private static readonly string InvalidUnaryEqualsFilter = "'<>'''";

        public override void Initialize(AnalysisContext context) =>
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeCalcFormula), SymbolKind.Field);

        private void AnalyzeCalcFormula(SymbolAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved())
                return;

            SyntaxNode? syntaxNode = ctx.Symbol.DeclaringSyntaxReference?.GetSyntax(ctx.CancellationToken);
            if (syntaxNode == null)
                return;

            if (syntaxNode is not FieldSyntax fieldSyntax)
                return;

            // Retrieve the 'CalcFormula' property from the field's property list
            var calcFormulaPropertySyntax = fieldSyntax.PropertyList?.Properties
                .OfType<PropertySyntax>()
                .Select(p => p.Value)
                .OfType<CalculationFormulaPropertyValueSyntax>()
                .FirstOrDefault();

            if (calcFormulaPropertySyntax is null)
                return;

            // Retrieve the filter expression from the 'Where' expression of the CalcFormula
            var filterExpressions = calcFormulaPropertySyntax.WhereExpression.Filter.Conditions
                .OfType<FilterExpressionSyntax>()
                .Where(c => c.Filter.Kind == SyntaxKind.UnaryEqualsFilterExpression)
                .Select(c => c.Filter);

            if (filterExpressions is null)
                return;

            foreach (var filter in filterExpressions)
            {
                if (filter.ToString().Equals(InvalidUnaryEqualsFilter))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0059SingleQuoteEscapingIssueDetected, filter.GetLocation()));
            }
        }
    }
}