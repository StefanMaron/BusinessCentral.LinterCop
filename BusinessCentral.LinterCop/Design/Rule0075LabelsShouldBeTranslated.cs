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
using System.Net.Http.Headers;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Text;

namespace CustomCodeCop;

[DiagnosticAnalyzer]
public class Rule0075LabelsShouldBeTranslated : DiagnosticAnalyzer
{
    private IEnumerable<XmlDocument>? xliffs;
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0075LabelsShouldBeTranslated);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterCompilationAction(new Action<CompilationAnalysisContext>(this.UpdateCache));
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(this.AnalyzeFlowFieldEditable),
            SymbolKind.Field,
            SymbolKind.LocalVariable,
            SymbolKind.GlobalVariable,
            SymbolKind.Parameter,
            SymbolKind.Option,
            SymbolKind.Table,
            SymbolKind.Page,
            SymbolKind.Report,
            SymbolKind.XmlPort,
            SymbolKind.EnumValue,
            SymbolKind.Query,
            SymbolKind.Profile,
            SymbolKind.Permission
        );
    }

    private void UpdateCache(CompilationAnalysisContext ctx)
    {
        UpdateCache(ctx.Compilation);
    }

    private void UpdateCache(Compilation compilation)
    {
        NavAppManifest? manifest = ManifestHelper.GetManifest(compilation);
        if (manifest == null)
        {
            return;
        }

        this.xliffs = new List<XmlDocument>();

        IFileSystem fileSystem = new FileSystem();

        IEnumerable<string> xliffFiles = LanguageFileUtilities.GetXliffLanguageFiles(fileSystem, manifest.AppName);

        foreach (string xliff in xliffFiles)
        {
            using (var stream = fileSystem.OpenRead(xliff))
            {
                var doc = new XmlDocument();
                doc.Load(stream);

                this.xliffs = this.xliffs.Append(doc);
            }
        }
    }

    private void AnalyzeFlowFieldEditable(SymbolAnalysisContext ctx)
    {
        List<Diagnostic?> diagnostics = new List<Diagnostic?>();

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

                diagnostics.Add(ReportDiagnostic(ctx.Symbol));
                break;

            case SymbolKind.Field:
                diagnostics.Add(ReportDiagnostic(ctx.Symbol.GetProperty(PropertyKind.Caption)));
                diagnostics.Add(ReportDiagnostic(ctx.Symbol.GetProperty(PropertyKind.ToolTip)));
                break;

            case SymbolKind.Page:
            case SymbolKind.PageExtension:
                diagnostics.Add(ReportDiagnostic(ctx.Symbol.GetProperty(PropertyKind.Caption)));

                IEnumerable<IControlSymbol> pageFields = GetFlattenedControls(ctx.Symbol)
                                                .Where(e => e.ControlKind == ControlKind.Field)
                                                .Where(e => e.GetProperty(PropertyKind.ToolTip) != null)
                                                .Where(e => e.RelatedFieldSymbol != null);

                foreach (IControlSymbol pageField in pageFields!)
                {
                    IPropertySymbol? pageToolTip = pageField.GetProperty(PropertyKind.ToolTip);
                    IPropertySymbol? pageCaption = pageField.GetProperty(PropertyKind.Caption);

                    if (pageToolTip != null)
                    {
                        diagnostics.Add(ReportDiagnostic(pageToolTip));
                    }
                    if(pageCaption != null)
                    {
                        diagnostics.Add(ReportDiagnostic(pageCaption));
                    }                    
                }

                break;

            case SymbolKind.Table:
            case SymbolKind.Report:
            case SymbolKind.XmlPort:
            case SymbolKind.EnumValue:
            case SymbolKind.Query:
            case SymbolKind.Profile:
            case SymbolKind.Permission:
                diagnostics.Add(ReportDiagnostic(ctx.Symbol.GetProperty(PropertyKind.Caption)));
                break;

            case SymbolKind.Option:
            case SymbolKind.Parameter:
                if (ctx.Symbol.Kind != SymbolKind.Option)
                {
                    return;
                }
                ;

                IOptionSymbol OptionString = (IOptionSymbol)ctx.Symbol;
                // todo: optiona values
                break;
            default:
                return;
        }

        foreach (Diagnostic diagnostic in diagnostics.Where(d => d != null).Cast<Diagnostic>().ToList())
        {
            ctx.ReportDiagnostic(diagnostic);
        }
    }

    private Diagnostic? ReportDiagnostic(ISymbol? label)
    {
        if (label == null)
        {
            return null;
        }

        string labelValue = LanguageFileUtilities.GetLanguageSymbolId(label, null);
        string languages = "";

        foreach (XmlDocument doc in this.xliffs ?? [])
        {
            languages += AnalyzeXML(doc, labelValue, label);
        }

        languages = languages.TrimStart(' ').TrimStart(',');

        if (languages != "")
        {
            return Diagnostic.Create(DiagnosticDescriptors.Rule0075LabelsShouldBeTranslated, label.Location, new object[] { label.Name, languages });
        }

        return null;
    }

    private static string AnalyzeXML(XmlDocument doc, string labelValue, ISymbol label)
    {
        XmlNode root = doc.DocumentElement;
        var nsManager = new XmlNamespaceManager(doc.NameTable);
        nsManager.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");

        // Find trans-unit by ID using XPath
        var transUnit = root.SelectSingleNode(
            $"//x:trans-unit[@id='{labelValue}']",
            nsManager
        );

        // Add XML namespace manager for XPath query
        nsManager.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");

        // Find trans-unit by ID using XPath
        var language = root.SelectSingleNode(
            $"//x:file/@target-language",
            nsManager
        ).InnerText;

        var location = label.Location;
        if (location == null)
        {
            return "";
        }

        if (transUnit == null)
        {
            return ", " + language;
        }
        else
        {
            var targetNode = transUnit.SelectSingleNode("x:target", nsManager);
            if (targetNode == null || string.IsNullOrEmpty(targetNode.InnerText) ||
                targetNode.Attributes["state"]?.Value == "needs-translation")
            {
                return ", " + language;
            }
        }
        return "";
    }
    private static IEnumerable<IControlSymbol>? GetFlattenedControls(ISymbol symbol)
    {
        switch (symbol.Kind)
        {
            case SymbolKind.Page:
                return ((IPageBaseTypeSymbol)symbol).FlattenedControls;
            case SymbolKind.PageExtension:
                return ((IPageExtensionBaseTypeSymbol)symbol).AddedControlsFlattened;
            default:
                return null;
        }
    }

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0075LabelsShouldBeTranslated = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0075",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0075LabelsShouldBeTranslatedTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0075LabelsShouldBeTranslatedFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0075LabelsShouldBeTranslatedDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0075");
    }
}
