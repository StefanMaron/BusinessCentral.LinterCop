using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0014PermissionSetCaptionLength : DiagnosticAnalyzer
    {
        private const int MAXCAPTIONLENGTH = 30;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0014PermissionSetCaptionLength);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.CheckPermissionSetNameAndCaptionLength), SymbolKind.PermissionSet);

        private void CheckPermissionSetNameAndCaptionLength(SymbolAnalysisContext context)
        {
            IPropertySymbol captionProperty = context.Symbol.GetProperty(PropertyKind.Caption);
            if (captionProperty == null)
            {
                return;
            }
            if (captionProperty?.ValueText.Length > MAXCAPTIONLENGTH)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0014PermissionSetCaptionLength, captionProperty.GetLocation(), new object[] { MAXCAPTIONLENGTH }));
            }
        }
    }
}
