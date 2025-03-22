namespace BusinessCentral.LinterCop.Test.Helpers;

internal static class CodeFixerTestHelpers
{
    private static readonly string BasePath = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
 
    public static async Task<string> GetCodeFixerTestCode(string className, params string[] folders)
    {
        return await File.ReadAllTextAsync(Path.Combine([BasePath, "CodeFixerTests", className, .. folders])).ConfigureAwait(false);
    }
}