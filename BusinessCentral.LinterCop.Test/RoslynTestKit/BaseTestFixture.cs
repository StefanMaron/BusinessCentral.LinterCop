using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;
using AdditionalText = Microsoft.Dynamics.Nav.CodeAnalysis.AdditionalText;
using CompilationOptions = Microsoft.Dynamics.Nav.CodeAnalysis.CompilationOptions;
using LanguageNames = Microsoft.Dynamics.Nav.CodeAnalysis.LanguageNames;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public abstract class BaseTestFixture
{
    protected abstract string LanguageName { get; }

    protected virtual bool ThrowsWhenInputDocumentContainsError { get; } = false;

    protected virtual IReadOnlyCollection<MetadataReference> References => null;

    protected virtual IReadOnlyCollection<AdditionalText> AdditionalFiles => null;

    protected Document CreateDocumentFromCode(string code)
    {
        return CreateDocumentFromCode(code, LanguageName, References ?? Array.Empty<MetadataReference>());
    }

    internal const string FileSeparator = "/*EOD*/";
    private readonly static Regex FileSeparatorPattern = new Regex(Regex.Escape(FileSeparator));


    /// <summary>
    ///     Should create the compilation and return a document that represents the provided code
    /// </summary>
    protected virtual Document CreateDocumentFromCode(string code, string languageName,
        IReadOnlyCollection<MetadataReference> extraReferences)
    {
        var frameworkReferences = CreateFrameworkMetadataReferences();

        var compilationOptions = GetCompilationOptions(languageName);

        var docs = FileSeparatorPattern.Split(code).Reverse().ToList();

        var project = new AdhocWorkspace()
            .AddProject("TestProject", languageName)
            .WithCompilationOptions(compilationOptions);
        //.AddMetadataReferences(frameworkReferences)
        //.AddMetadataReferences(extraReferences);

        Document mainDocument = null;
        foreach (var doc in docs.Select((e, i) => (e, i)))
        {
            var docContent = docs.Count > 1 ? doc.e.Trim() : doc.e;
            mainDocument = project.AddDocument($"TestDocument{doc.i}", docContent);
            project = mainDocument.Project;
        }

        return mainDocument;
    }

    private static CompilationOptions GetCompilationOptions(string languageName) =>
        languageName switch
        {
            LanguageNames.AL => new CompilationOptions(),
            //LanguageNames.CSharp => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            //LanguageNames.VisualBasic => new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            _ => throw new NotSupportedException($"Language {languageName} is not supported")
        };

    protected virtual IEnumerable<MetadataReference?> CreateFrameworkMetadataReferences()
    {
        yield return ReferenceSource.Core;
        yield return ReferenceSource.Linq;
        yield return ReferenceSource.LinqExpression;

        if (ReferenceSource.Core.Display.EndsWith("mscorlib.dll") == false)
        {
            foreach (var netStandardCoreLib in ReferenceSource.NetStandardBasicLibs.Value)
            {
                yield return netStandardCoreLib;
            }
        }
    }
}
