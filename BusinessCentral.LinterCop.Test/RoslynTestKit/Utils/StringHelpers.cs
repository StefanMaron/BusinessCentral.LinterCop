namespace BusinessCentral.LinterCop.Test.RoslynTestKit.Utils;

internal static class StringHelpers
{
    public static string MergeWithComma<T>(this IReadOnlyList<T> list, Func<T, string> map = null, string title = null)
    {
        return MergeWith(list, map, title, ", ");
    }

    public static string MergeWithNewLines<T>(this IReadOnlyList<T> list, Func<T, string> map = null,
        string title = null)
    {
        return MergeWith(list, map, title, Environment.NewLine);
    }

    private static string MergeWith<T>(IReadOnlyList<T> list, Func<T, string> map, string title, string separator)
    {
        if (list.Count == 0)
        {
            return string.Empty;
        }

        return title + string.Join(separator, list.Select(map ?? (x => x.ToString())));
    }

    public static string MergeAsBulletList<T>(this IReadOnlyList<T> list, Func<T, string> map = null,
        string title = null)
    {
        if (list.Count == 0)
        {
            return string.Empty;
        }

        var mapItem = map ?? (x => x.ToString());
        return title + string.Join(Environment.NewLine, list.Select(x => $"- {mapItem(x)}"));
    }
}