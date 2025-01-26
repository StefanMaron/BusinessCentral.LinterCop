using Microsoft.Dynamics.Nav.Analyzers.Common;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Packaging;
using Microsoft.Dynamics.Nav.CodeAnalysis.Translation;
using System.Collections.Immutable;
using BusinessCentral.LinterCop.AnalysisContextExtension;
using System.Xml;
using BusinessCentral.LinterCop;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;

namespace CustomCodeCop;

[DiagnosticAnalyzer]
public class Rule0088LabelsShouldBeTranslated : DiagnosticAnalyzer
{
    public Rule0088LabelsShouldBeTranslated()
    {
        xliffs = new List<XmlDocument>();
    }
    private List<XmlDocument> xliffs;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0088LabelsShouldBeTranslated);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterCompilationAction(new Action<CompilationAnalysisContext>(UpdateCache));
        context.RegisterSymbolAction(new Action<SymbolAnalysisContext>(AnalyzeLabelTranslation),
            SymbolKind.Field,
            SymbolKind.LocalVariable,
            SymbolKind.GlobalVariable,
            SymbolKind.Table,
            SymbolKind.TableExtension,
            SymbolKind.Page,
            SymbolKind.PageExtension,
            SymbolKind.Report,
            SymbolKind.XmlPort,
            SymbolKind.EnumValue,
            SymbolKind.Query, //TODO: daitem captions
            SymbolKind.Profile,
            SymbolKind.PermissionSet,
            SymbolKind.Action, //TODO: page actions
            SymbolKind.RequestPage,
            SymbolKind.RequestPageExtension,
            SymbolKind.ReportLabel
        );
    }

    private void UpdateCache(CompilationAnalysisContext ctx)
    {
        UpdateCache(ctx.Compilation);
    }

    private void UpdateCache(Compilation compilation)
    {
        this.xliffs = new List<XmlDocument>();

        NavAppManifest? manifest = ManifestHelper.GetManifest(compilation);
        if (manifest == null) return;
        if (!manifest.CompilerFeatures.ShouldGenerateTranslationFile()) return;


        IFileSystem fileSystem = new FileSystem();
        IEnumerable<string> xliffFiles = Enumerable.Empty<string>();

        try
        {
            xliffFiles = LanguageFileUtilities.GetXliffLanguageFiles(fileSystem, manifest.AppName);
        }
        catch (DirectoryNotFoundException)
        {
            return; // no Translations folder exists
        }

        foreach (string xliff in xliffFiles)
        {
            using (var stream = fileSystem.OpenRead(xliff))
            {
                var doc = new XmlDocument();
                doc.Load(stream);

                this.xliffs.Add(doc);
            }
        }
    }

    private void AnalyzeLabelTranslation(SymbolAnalysisContext ctx)
    {
        List<Diagnostic?> diagnostics = new List<Diagnostic?>();

        switch (ctx.Symbol.Kind)
        {
            case SymbolKind.LocalVariable:
            case SymbolKind.GlobalVariable:
                IVariableSymbol symbol = (IVariableSymbol)ctx.Symbol;

                if (symbol.Type.NavTypeKind != NavTypeKind.Label) return;

                diagnostics.Add(ReportDiagnostic(ctx.Symbol));
                break;

            case SymbolKind.ReportLabel:
                diagnostics.Add(ReportDiagnostic(ctx.Symbol));
                break;

            case SymbolKind.Field:
                diagnostics.Add(ReportDiagnostic(ctx.Symbol.GetProperty(PropertyKind.Caption)));
                diagnostics.Add(ReportDiagnostic(ctx.Symbol.GetProperty(PropertyKind.ToolTip)));
                break;

            case SymbolKind.Page:
            case SymbolKind.PageExtension:
            case SymbolKind.Action:
            case SymbolKind.RequestPageExtension:
            case SymbolKind.RequestPage:
            case SymbolKind.Query:
                diagnostics.Add(ReportDiagnostic(ctx.Symbol.GetProperty(PropertyKind.Caption)));

                IEnumerable<IControlSymbol>? flattenedControls = GetFlattenedControls(ctx.Symbol)?.
                            Where(e => e.GetProperty(PropertyKind.ToolTip) != null || e.GetProperty(PropertyKind.Caption) != null);

                foreach (IControlSymbol flattenedControl in flattenedControls ?? [])
                {
                    IPropertySymbol? optionCaption = flattenedControl.GetProperty(PropertyKind.OptionCaption);
                    if (optionCaption != null) diagnostics.Add(ReportDiagnostic(optionCaption));

                    IPropertySymbol? toolTip = flattenedControl.GetProperty(PropertyKind.ToolTip);
                    if (toolTip != null) diagnostics.Add(ReportDiagnostic(toolTip));

                    IPropertySymbol? caption = flattenedControl.GetProperty(PropertyKind.Caption);
                    if (caption != null) diagnostics.Add(ReportDiagnostic(caption));

                    IPropertySymbol? groupName = flattenedControl.GetProperty(PropertyKind.GroupName);
                    if (groupName != null) diagnostics.Add(ReportDiagnostic(groupName));
                }
                break;

            case SymbolKind.Table:
            case SymbolKind.XmlPort:
            case SymbolKind.EnumValue:
            case SymbolKind.Profile:
            case SymbolKind.Report:
            case SymbolKind.PermissionSet:
                diagnostics.Add(ReportDiagnostic(ctx.Symbol.GetProperty(PropertyKind.Caption)));
                break;

            default:
                return;
        }

        diagnostics.Where(d => d != null).Cast<Diagnostic>().ToList().ForEach(ctx.ReportDiagnostic);
    }

    private Diagnostic? ReportDiagnostic(ISymbol? label)
    {
        if (label == null) return null;
        if (label.IsObsoletePendingOrRemoved()) return null;
        if(LabelIsLocked(label)) return null;

        string labelValue = LanguageFileUtilities.GetLanguageSymbolId(label, null);
        string languages = "";

        foreach (XmlDocument doc in this.xliffs ?? Enumerable.Empty<XmlDocument>())
        {
            languages += AnalyzeXML(doc, labelValue, label);
        }

        languages = languages.TrimStart(' ').TrimStart(',');

        if (languages != "")
        {
            return Diagnostic.Create(DiagnosticDescriptors.Rule0088LabelsShouldBeTranslated, label.GetLocation(), new object[] { label.Name, languages });
        }

        return null;
    }

    private bool LabelIsLocked(ISymbol label)
    {
        IEnumerable<SyntaxNode> subProperties;

        // checks local and global Label variables
        if (label.GetTypeSymbol() is ILabelTypeSymbol labelTypeSymbol)
        {
            if (labelTypeSymbol.Locked) return true;
        }
        else
        {
            // checks syntax nodes like Page.Caption, Page.ToolTip, ReportLabels
            if (label is IPropertySymbol) subProperties = ExtractSubProperties(((IPropertySymbol)label).DeclaringSyntaxReference);
            else if (label is IReportLabelSymbol) subProperties = ExtractSubProperties(((IReportLabelSymbol)label).DeclaringSyntaxReference);
            else return true;

            if (subProperties is null || subProperties.Any(node => node.ToString().Contains("Locked", StringComparison.OrdinalIgnoreCase)))
                return true;
        }

        return false;
    }

    private IEnumerable<SyntaxNode> ExtractSubProperties(SyntaxReference? syntaxReference)
    {
        if (syntaxReference is null)
            return Enumerable.Empty<SyntaxNode>();

        var syntaxNode = syntaxReference.GetSyntax();
        if (syntaxNode is null)
            return Enumerable.Empty<SyntaxNode>();

        var subPropertyNode = syntaxNode.DescendantNodes()
            .FirstOrDefault(e => e.Kind == SyntaxKind.CommaSeparatedIdentifierEqualsLiteralList);

        return subPropertyNode?.DescendantNodes() ?? Enumerable.Empty<SyntaxNode>();
    }
    private static string AnalyzeXML(XmlDocument doc, string labelValue, ISymbol label)
    {
        XmlNode root = doc.DocumentElement;
        var nsManager = new XmlNamespaceManager(doc.NameTable);

        // Find translation unit
        nsManager.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");
        var transUnitNode = root.SelectSingleNode($"//x:trans-unit[@id='{labelValue}']", nsManager);

        // find target language
        nsManager.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");
        var language = root.SelectSingleNode($"//x:file/@target-language", nsManager)?.InnerText ?? string.Empty;

        if (transUnitNode == null && language != string.Empty)
        {
            return $",{language}";
        }
        else
        {
            var targetNode = transUnitNode?.SelectSingleNode("x:target", nsManager);
            if (targetNode == null || string.IsNullOrEmpty(targetNode.InnerText) ||
                targetNode.Attributes["state"]?.Value == "needs-translation")
            {
                return $",{language}";
            }
        }

        return string.Empty;
    }
    static IEnumerable<IControlSymbol>? GetFlattenedControls(ISymbol symbol) =>
        symbol switch
        {
            IPageBaseTypeSymbol page => page.FlattenedControls,
            IPageExtensionBaseTypeSymbol pageExtension => pageExtension.AddedControlsFlattened,
            IRequestPageExtensionTypeSymbol requestPageExtension => requestPageExtension.AddedControlsFlattened,
            _ => null
        };

    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor Rule0088LabelsShouldBeTranslated = new(
            id: LinterCopAnalyzers.AnalyzerPrefix + "0088",
            title: LinterCopAnalyzers.GetLocalizableString("Rule0088LabelsShouldBeTranslatedTitle"),
            messageFormat: LinterCopAnalyzers.GetLocalizableString("Rule0088LabelsShouldBeTranslatedFormat"),
            category: "Design",
            defaultSeverity: DiagnosticSeverity.Info, isEnabledByDefault: true,
            description: LinterCopAnalyzers.GetLocalizableString("Rule0088LabelsShouldBeTranslatedDescription"),
            helpLinkUri: "https://github.com/StefanMaron/BusinessCentral.LinterCop/wiki/LC0088");
    }
}
