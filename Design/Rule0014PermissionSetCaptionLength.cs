﻿using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

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
                return;

            if (captionProperty?.ValueText.Length > MAXCAPTIONLENGTH)
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0014PermissionSetCaptionLength, captionProperty.GetLocation(), new object[] { MAXCAPTIONLENGTH }));

            var captionSubProperties = captionProperty.DeclaringSyntaxReference.GetSyntax().DescendantNodes(e => true).FirstOrDefault(e => e.Kind == SyntaxKind.CommaSeparatedIdentifierEqualsLiteralList);
            if (captionSubProperties != null)
            {
                if (captionSubProperties.DescendantNodes().Any(e => e.ToString().StartsWith("Locked")))
                    return;

                var maxLengthProperty = captionSubProperties.DescendantNodes().FirstOrDefault(e => e.ToString().StartsWith("MaxLength"));
                if (captionSubProperties.ToString() != "")
                {
                    if (Int32.TryParse(maxLengthProperty.DescendantNodes().FirstOrDefault(e => e.Kind == SyntaxKind.Int32SignedLiteralValue).ToString(), out int maxLengthValue))
                        if (maxLengthValue > MAXCAPTIONLENGTH)
                            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0014PermissionSetCaptionLength, captionProperty.GetLocation(), new object[] { MAXCAPTIONLENGTH }));
                }
            }
            else
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0014PermissionSetCaptionLength, captionProperty.GetLocation(), new object[] { MAXCAPTIONLENGTH }));
        }
    }
}
