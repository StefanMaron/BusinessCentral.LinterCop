using Microsoft.Dynamics.Nav.CodeAnalysis.CodeActions;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit.CodeActionLocators;

/// <summary>
///     This selector is intended to search for a nested code actions
/// </summary>
public class ByTitleForNestedActionSelector : ICodeActionSelector
{
    private readonly ByTitleCodeActionSelector groupSelector;
    private readonly ByTitleCodeActionSelector nestedActionSelector;

    public ByTitleForNestedActionSelector(string groupTitle, string actionTitle)
    {
        groupSelector = new ByTitleCodeActionSelector(groupTitle);
        nestedActionSelector = new ByTitleCodeActionSelector(actionTitle);
    }

    public CodeAction Find(IReadOnlyList<CodeAction> actions)
    {
        if (groupSelector.Find(actions) is { } group && NestedCodeActionHelper.TryGetNestedAction(group) is
                { } nestedActions)
        {
            return nestedActionSelector.Find(nestedActions);
        }

        return null;
    }

    public override string ToString() => $"with nested action [{groupSelector}] -> [{nestedActionSelector}]";
}