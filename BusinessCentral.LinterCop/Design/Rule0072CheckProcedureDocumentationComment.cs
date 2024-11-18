using BusinessCentral.LinterCop.AnalysisContextExtension;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;
using System.Collections.Immutable;

namespace BusinessCentral.LinterCop.Design
{
    [DiagnosticAnalyzer]
    public class Rule0072CheckProcedureDocumentationComment : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0072CheckProcedureDocumentationComment);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(new Action<SyntaxNodeAnalysisContext>(this.AnalyzeDocumentationComments), SyntaxKind.MethodDeclaration);

        private void AnalyzeDocumentationComments(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.IsObsoletePendingOrRemoved()) return;

            if (ctx.Node is not MethodDeclarationSyntax methodDeclarationSyntax)
                return;
            var docCommentTrivia = methodDeclarationSyntax.GetLeadingTrivia().FirstOrDefault(trivia => trivia.Kind == SyntaxKind.SingleLineDocumentationCommentTrivia);
            if (docCommentTrivia.IsKind(SyntaxKind.None)) return; // no documentation comment exists

            Dictionary<string, XmlElementSyntax> docCommentParameters = new Dictionary<string, XmlElementSyntax>(StringComparer.OrdinalIgnoreCase);
            XmlElementSyntax? docCommentReturns = null;

            var docCommentStructure = (DocumentationCommentTriviaSyntax)docCommentTrivia.GetStructure();
            var docCommentElements = docCommentStructure.Content.Where(xmlNode => xmlNode.Kind == SyntaxKind.XmlElement);

            // evaluate documentation comment syntax
            foreach (XmlElementSyntax element in docCommentElements.Cast<XmlElementSyntax>())
            {
                switch (element.StartTag.Name.LocalName.Text.ToLowerInvariant())
                {
                    case "param":
                        var nameAttribute = (XmlNameAttributeSyntax)element.StartTag.Attributes.First(att => att.IsKind(SyntaxKind.XmlNameAttribute));
                        var parameterName = nameAttribute.Identifier.Identifier.ValueText;
                        if (!docCommentParameters.ContainsKey(parameterName))
                            docCommentParameters.Add(parameterName, element);
                        break;
                    case "returns":
                        docCommentReturns = element;
                        break;
                }
            }

            // excess documentation comment return value
            if (docCommentReturns is not null && methodDeclarationSyntax.ReturnValue is null)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0072CheckProcedureDocumentationComment, docCommentReturns.GetLocation()));

            // return value without documentation comment
            if (docCommentReturns is null && methodDeclarationSyntax.ReturnValue is not null)
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0072CheckProcedureDocumentationComment, methodDeclarationSyntax.ReturnValue.GetLocation()));

            // check documentation comment parameters against method syntax
            foreach (var docCommentParameter in docCommentParameters)
            {
                if (!methodDeclarationSyntax.ParameterList.Parameters.Any(param => param.Name.Identifier.ValueText.UnquoteIdentifier().ToLowerInvariant() == docCommentParameter.Key))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0072CheckProcedureDocumentationComment, docCommentParameter.Value.GetLocation()));
            }

            // check method parameters against documentation comment syntax
            foreach (var methodParameter in methodDeclarationSyntax.ParameterList.Parameters)
            {
                if (!docCommentParameters.Any(docParam => docParam.Key == methodParameter.Name.Identifier.ValueText.UnquoteIdentifier().ToLowerInvariant()))
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0072CheckProcedureDocumentationComment, methodParameter.GetLocation()));
            }
        }
    }
}