using Microsoft.Dynamics.Nav.Analyzers.Common;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Packaging;
using Microsoft.Dynamics.Nav.CodeAnalysis.Translation;
using System.Collections.Immutable;
using System.Xml;
using BusinessCentral.LinterCop;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Symbols;
using BusinessCentral.LinterCop.Helpers;
using Microsoft.Dynamics.Nav.Analyzers.Common.AppSourceCopConfiguration;

namespace BusinessCentral.LinterCop.Design;

[DiagnosticAnalyzer]
public class Rule0091LabelsShouldBeTranslated : DiagnosticAnalyzer
{
    public Rule0091LabelsShouldBeTranslated()
    {
        this.translationIndex = new Dictionary<string, HashSet<string>>();
        this.availableLanguages = new HashSet<string>();
    }
    private Dictionary<string, HashSet<string>> translationIndex = new Dictionary<string, HashSet<string>>();
    private HashSet<string> availableLanguages = new HashSet<string>();
    public bool DoNotUpdateCache { get; set; } = false;


    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create<DiagnosticDescriptor>(DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated);

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
            SymbolKind.Enum,
            SymbolKind.EnumValue,
            SymbolKind.Query, //TODO: daitem captions
            SymbolKind.Profile,
            SymbolKind.PermissionSet,
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
        if (DoNotUpdateCache) return;

        IEnumerable<Stream> xliffFileStream;
        xliffFileStream = this.ReadXliffFiles(compilation);
        UpdateCache(xliffFileStream);
    }

    public void UpdateCache(IEnumerable<Stream> xliffFileStream)
    {
        if (DoNotUpdateCache) return;

        this.translationIndex = new Dictionary<string, HashSet<string>>();
        this.availableLanguages = new HashSet<string>();
        var docs = new List<XmlDocument>();

        foreach (var stream in xliffFileStream)
        {
            using (stream)
            {
                var doc = new XmlDocument();
                doc.Load(stream);
                docs.Add(doc);

                var nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");

                string language = doc.SelectSingleNode("//x:file/@target-language", nsManager)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(language))
                    continue;

                this.availableLanguages.Add(language);
            }
        }

        foreach (XmlDocument doc in docs)
        {
            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");

            string language = doc.SelectSingleNode("//x:file/@target-language", nsManager)?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(language))
                continue;

            HashSet<string> languageHashSet = new HashSet<string> { language };

            // Create a set of found IDs for this language
            XmlNodeList? transUnits = doc.SelectNodes("//x:trans-unit", nsManager);
            if (transUnits == null)
                continue;

            foreach (XmlNode transUnit in transUnits)
            {
                string? id = transUnit.Attributes?["id"]?.Value;
                if (string.IsNullOrEmpty(id))
                    continue;

                XmlNode? targetNode = transUnit.SelectSingleNode("x:target", nsManager);
                bool missingTranslation = targetNode == null ||
                                                              string.IsNullOrWhiteSpace(targetNode.InnerText) ||
                                                              (targetNode.Attributes?["state"]?.Value == "needs-translation");
                if (!this.translationIndex.TryGetValue(id, out _))
                {
                    this.translationIndex[id] = [.. availableLanguages];
                }

                if (!missingTranslation)
                {
                    this.translationIndex[id].ExceptWith(languageHashSet);
                }
            }
        }
    }

    private IEnumerable<Stream> ReadXliffFiles(Compilation compilation)
    {
        IEnumerable<Stream> xliffFileStream = [];
        IFileSystem fileSystem = new FileSystem();

#if !LessThenSpring2024
        NavAppManifest? manifest = ManifestHelper.GetManifest(compilation);
#else
        NavAppManifest? manifest = AppSourceCopConfigurationProvider.GetManifest(compilation);
#endif

        if (manifest == null) return xliffFileStream;
        if (!manifest.CompilerFeatures.ShouldGenerateTranslationFile()) return xliffFileStream;

        xliffFileStream = Enumerable.Empty<Stream>();
        IEnumerable<string> xliffFiles = Enumerable.Empty<string>();

        try
        {
            xliffFiles = LanguageFileUtilities.GetXliffLanguageFiles(fileSystem, manifest.AppName);
        }
        catch (DirectoryNotFoundException)
        {
            return xliffFileStream; // no Translations folder exists
        }

        foreach (string xliff in xliffFiles)
        {
            xliffFileStream = xliffFileStream.Append(fileSystem.OpenRead(xliff));
        }

        return xliffFileStream;
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
            case SymbolKind.RequestPageExtension:
            case SymbolKind.RequestPage:
            case SymbolKind.Query:
                diagnostics.Add(ReportDiagnostic(ctx.Symbol.GetProperty(PropertyKind.Caption)));

                IEnumerable<IControlSymbol>? flattenedControls = GetFlattenedControls(ctx.Symbol)?.
                            Where(e => e.GetProperty(PropertyKind.ToolTip) != null || e.GetProperty(PropertyKind.Caption) != null);

                IEnumerable<IActionSymbol>? flattenedActions = GetFlattenedActions(ctx.Symbol)?.
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

                foreach (IActionSymbol flattenedAction in flattenedActions ?? [])
                {
                    IPropertySymbol? toolTip = flattenedAction.GetProperty(PropertyKind.ToolTip);
                    if (toolTip != null) diagnostics.Add(ReportDiagnostic(toolTip));

                    IPropertySymbol? caption = flattenedAction.GetProperty(PropertyKind.Caption);
                    if (caption != null) diagnostics.Add(ReportDiagnostic(caption));
                }
                break;

            case SymbolKind.Table:
            case SymbolKind.TableExtension:
            case SymbolKind.XmlPort:
            case SymbolKind.EnumValue:
            case SymbolKind.Enum:
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
        if (label == null)
            return null;
        if (label.ContainingSymbol.IsObsoletePendingOrRemoved())
            return null;
        if (LabelIsLocked(label))
            return null;

        string labelValue = GetLanguageSymbolId(label);

        // If there are no languages available, nothing to report
        if (this.availableLanguages.Count == 0)
            return null;

        HashSet<string> missingLanguages;
        if (!this.translationIndex.TryGetValue(labelValue, out missingLanguages))
        {
            // No entry found in the index means the label isn't present in any translation file
            missingLanguages = new HashSet<string>(this.availableLanguages);
        }
        else
        {
            // Only report languages that are available and missing
            missingLanguages = missingLanguages.Intersect(this.availableLanguages).ToHashSet();
        }

        if (missingLanguages.Count > 0)
        {
            string languages = string.Join(",", missingLanguages.OrderBy(lang => lang));
            return Diagnostic.Create(
                DiagnosticDescriptors.Rule0091LabelsShouldBeTranslated,
                label.GetLocation(),
                new object[] { label.Name, languages });
        }

        return null;
    }

    internal string GetLanguageSymbolId(ISymbol labelSymbol)
    {
        PooledList<string> instance = PooledList<string>.GetInstance();

        ISymbol? rootSymbol = labelSymbol;
        ISymbol? containingSymbol = labelSymbol.ContainingSymbol;

        try
        {
            while (rootSymbol.ContainingSymbol != null && rootSymbol is not IRootTypeSymbol)
            {
                rootSymbol = rootSymbol.ContainingSymbol;
            }

            instance.Add(GetLabelName(rootSymbol));

            if (rootSymbol != containingSymbol?.ContainingSymbol && containingSymbol?.Kind == SymbolKind.Method) 
            {
                rootSymbol = containingSymbol.ContainingSymbol;
                instance.Add(GetLabelName(containingSymbol.ContainingSymbol));
            }

            if (rootSymbol != containingSymbol) 
            {
                rootSymbol = containingSymbol;
                instance.Add(GetLabelName(rootSymbol));
            }

            instance.Add(GetLabelName(labelSymbol));
            return MakeSymbolIdString(instance);
        }
        finally
        {
            instance.Free();
        }
    }

    private string GetLabelName(ISymbol? label) =>
        label switch
        {
            IPageExtensionTypeSymbol page => page.Target?.Kind.ToString() + " " + LanguageFileUtilities.GetNameHash(page.Target?.Name ?? string.Empty),
            ITableExtensionTypeSymbol table => table.Target?.Kind.ToString() + " " + LanguageFileUtilities.GetNameHash(table.Target?.Name ?? string.Empty),

#if !LessThenSpring2024
            IReportExtensionTypeSymbol report => report.Target?.Kind.ToString() + " " + LanguageFileUtilities.GetNameHash(report.Target?.Name ?? string.Empty),
#endif

            _ => $"{(label?.Kind is SymbolKind.GlobalVariable or SymbolKind.LocalVariable ? SymbolKind.NamedType : label?.Kind)} {LanguageFileUtilities.GetNameHash(label?.Name ?? string.Empty)}"
        };

    private string MakeSymbolIdString(IList<string> ids)
    {
        string objectId = "";

        for (int i = 0; i < ids.Count; i++)
        {
            objectId += " - " + ids[i];
        }

        return objectId.TrimStart(' ', '-');
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

    static IEnumerable<IControlSymbol>? GetFlattenedControls(ISymbol symbol) =>
        symbol switch
        {
            IPageBaseTypeSymbol page => page.FlattenedControls,
            IPageExtensionBaseTypeSymbol pageExtension => pageExtension.AddedControlsFlattened,
            IRequestPageExtensionTypeSymbol requestPageExtension => requestPageExtension.AddedControlsFlattened,
            _ => null
        };

    static IEnumerable<IActionSymbol>? GetFlattenedActions(ISymbol symbol) =>
        symbol switch
        {
            IPageBaseTypeSymbol page => page.FlattenedActions,
            IPageExtensionBaseTypeSymbol pageExtension => pageExtension.AddedActionsFlattened,
            _ => null
        };
}
