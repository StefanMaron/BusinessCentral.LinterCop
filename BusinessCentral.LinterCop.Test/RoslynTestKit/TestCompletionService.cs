using System.Collections.Immutable;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces;
using Microsoft.Dynamics.Nav.CodeAnalysis.Workspaces.Completion;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

internal class TestCompletionService : CompletionServiceWithProviders
{
    public TestCompletionService(Workspace workspace, CompletionProvider provider)
        : base(workspace)
    {
        TestProviders = [..new[] { provider }];
    }

    public ImmutableArray<CompletionProvider> TestProviders { get; }

    protected override ImmutableArray<CompletionProvider> GetProviders(ImmutableHashSet<string> roles,
        CompletionTrigger trigger)
    {
        return TestProviders;
    }
}