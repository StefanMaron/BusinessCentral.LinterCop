#nullable disable // TODO: Enable nullable and review rule
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0019DataClassificationFieldEqualsTable : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0019DataClassificationFieldEqualsTable);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(CheckDataClassificationRedundancy), SymbolKind.Field);
        }

        private void CheckDataClassificationRedundancy(SymbolAnalysisContext symbolAnalysisContext)
        {
            IFieldSymbol Field = (IFieldSymbol)symbolAnalysisContext.Symbol;
            if (Field == null || Field.IsObsoleteRemoved || Field.IsObsoletePending)
                return;

            IApplicationObjectTypeSymbol applicationObject = Field.GetContainingApplicationObjectTypeSymbol();
            if (!(applicationObject is ITableTypeSymbol) || applicationObject.IsObsoleteRemoved || applicationObject.IsObsoletePending)
                return;

            ITableTypeSymbol Table = (ITableTypeSymbol)Field.ContainingSymbol;
            IPropertySymbol fieldClassification = Field.GetProperty(PropertyKind.DataClassification) as IPropertySymbol;
            IPropertySymbol tableClassification = Table.GetProperty(PropertyKind.DataClassification) as IPropertySymbol;

            if (fieldClassification == null || tableClassification == null)
                return;

            if (fieldClassification.ValueText == tableClassification.ValueText)
                symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0019DataClassificationFieldEqualsTable, fieldClassification.GetLocation()));
        }

    }
}
