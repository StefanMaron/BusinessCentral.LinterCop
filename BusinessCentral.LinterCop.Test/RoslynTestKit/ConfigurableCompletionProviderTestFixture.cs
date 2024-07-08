using Microsoft.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces.Completion;
using AdditionalText = Microsoft.Dynamics.Nav.CodeAnalysis.AdditionalText;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

internal class ConfigurableCompletionProviderTestFixture : CompletionProviderFixture
{
    private readonly CompletionProviderTestFixtureConfig _config;
    private readonly CompletionProvider _provider;

    public ConfigurableCompletionProviderTestFixture(CompletionProvider provider,
        CompletionProviderTestFixtureConfig config)
    {
        _config = config;
        _provider = provider;
    }

    protected override string LanguageName => _config.Language;
    protected override CompletionProvider CreateProvider() => _provider;
    protected override IReadOnlyCollection<MetadataReference> References => _config.References;
    protected override IReadOnlyCollection<AdditionalText> AdditionalFiles => _config.AdditionalFiles;
    protected override bool ThrowsWhenInputDocumentContainsError => _config.ThrowsWhenInputDocumentContainsError;
}