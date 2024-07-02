using Microsoft.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CodeRefactoring;
using AdditionalText = Microsoft.Dynamics.Nav.CodeAnalysis.AdditionalText;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

internal class ConfigurableCodeRefactoringTestFixture : CodeRefactoringTestFixture
{
    private readonly CodeRefactoringTestFixtureConfig _config;
    private readonly CodeRefactoringProvider _provider;

    public ConfigurableCodeRefactoringTestFixture(CodeRefactoringProvider provider,
        CodeRefactoringTestFixtureConfig config)
    {
        _config = config;
        _provider = provider;
    }

    protected override string LanguageName => _config.Language;
    protected override CodeRefactoringProvider CreateProvider() => _provider;
    protected override IReadOnlyCollection<MetadataReference> References => _config.References;
    protected override IReadOnlyCollection<AdditionalText> AdditionalFiles => _config.AdditionalFiles;
    protected override bool ThrowsWhenInputDocumentContainsError => _config.ThrowsWhenInputDocumentContainsError;
}