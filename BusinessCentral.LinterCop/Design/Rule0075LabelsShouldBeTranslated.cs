using Microsoft.Dynamics.Nav.Analyzers.Common;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Packaging;
using Microsoft.Dynamics.Nav.CodeAnalysis.SymbolReference;
using Microsoft.Dynamics.Nav.CodeAnalysis.Translation;
using Microsoft.Dynamics.Nav.CodeAnalysis.Translation.LanguageFile;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.AnalysisContextExtension;
using System.Xml;
using BusinessCentral.LinterCop;

namespace CustomCodeCop;

[DiagnosticAnalyzer]
public class Rule0075LabelsShouldBeTranslated : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0075LabelsShouldBeTranslated);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeFlowFieldEditable), SymbolKind.Field, SymbolKind.LocalVariable, SymbolKind.GlobalVariable);
    }

    private void AnalyzeFlowFieldEditable(SymbolAnalysisContext ctx)
    {
        ISymbol? label;

        switch (ctx.Symbol.Kind)
        {
            case SymbolKind.LocalVariable:
            case SymbolKind.GlobalVariable:
                IVariableSymbol symbol = (IVariableSymbol)ctx.Symbol;
                ITypeSymbol type = symbol.Type;

                if (type == null || type.NavTypeKind != NavTypeKind.Label)
                {
                    return;
                }
                
                label = ctx.Symbol;
                break;
            case SymbolKind.Field:
                label = ctx.Symbol.GetProperty(PropertyKind.Caption);
                break;
            default:
                return;
        }

        if (label == null)
        {
            return;
        }

        string labelValue = LanguageFileUtilities.GetLanguageSymbolId(label, null);
        IFileSystem fileSystem = new FileSystem();

        NavAppManifest? manifest = ManifestHelper.GetManifest(ctx.Compilation);
        if (manifest == null)
        {
            return;
        }

        IEnumerable<string> xliffs = LanguageFileUtilities.GetXliffLanguageFiles(fileSystem, manifest.AppName);

        fileSystem.OpenRead(xliffs.First());
        using (var stream = fileSystem.OpenRead(xliffs.First()))
        {
            var doc = new XmlDocument();
            doc.Load(stream);
            XmlNode root = doc.DocumentElement;

            // Add XML namespace manager for XPath query
            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");

            // Find trans-unit by ID using XPath
            var transUnit = root.SelectSingleNode(
                $"//x:trans-unit[@id='{labelValue}']",
                nsManager
            );

            if (transUnit == null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0075LabelsShouldBeTranslated, label.Location, label.Name));
            }
            else
            {
                var targetNode = transUnit.SelectSingleNode("x:target", nsManager);
                if (targetNode == null || string.IsNullOrEmpty(targetNode.InnerText) ||
                    targetNode.Attributes["state"]?.Value == "needs-translation")
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule0075LabelsShouldBeTranslated, label.Location, label.Name));
                }
            }
        }
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0075LabelsShouldBeTranslated = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0074",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0074FlowFilterAssignmentTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0074FlowFilterAssignmentFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0074FlowFilterAssignmentDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0075");
    }
}
