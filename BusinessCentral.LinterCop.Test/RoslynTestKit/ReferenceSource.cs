using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace BusinessCentral.LinterCop.Test.RoslynTestKit;

public static class ReferenceSource
{
    internal static readonly MetadataReference? Core = FromType<int>();
    internal static readonly MetadataReference? Linq = FromType(typeof(Enumerable));
    internal static readonly MetadataReference? LinqExpression = FromType(typeof(Expression));
    private static readonly string[] NetCoreAssemblies;
    public static readonly MetadataReference? NetStandardCore;

    static ReferenceSource()
    {
        var trustedPlatformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        if (trustedPlatformAssemblies != null)
        {
            NetCoreAssemblies = ((String)trustedPlatformAssemblies)?.Split(Path.PathSeparator);
            NetStandardCore =
                MetadataReference.CreateFromFile(NetCoreAssemblies.FirstOrDefault(x => x.EndsWith("mscorlib.dll")));
        }
        else
        {
            NetStandardCore = null;
        }
    }

    internal static readonly Lazy<IReadOnlyList<MetadataReference?>> NetStandardBasicLibs =
        new Lazy<IReadOnlyList<MetadataReference?>>(() => GetNetStandardCoreLibs().ToList());

    internal static IEnumerable<MetadataReference?> GetNetStandardCoreLibs()
    {
        if (NetStandardCore == null) yield break;
        yield return NetStandardCore;

        var mscorlibFile = NetCoreAssemblies.FirstOrDefault(x => x.EndsWith("mscorlib.dll"));
        if (string.IsNullOrWhiteSpace(mscorlibFile)) yield break;
        var referencedAssemblies = Assembly.LoadFile(mscorlibFile).GetReferencedAssemblies();
        foreach (var referencedAssembly in referencedAssemblies)
        {
            var assemblyFile = NetCoreAssemblies.FirstOrDefault(x => x.EndsWith($"{referencedAssembly.Name}.dll"));
            if (string.IsNullOrWhiteSpace(assemblyFile) == false)
            {
                yield return MetadataReference.CreateFromFile(assemblyFile);
            }
        }
    }

    public static MetadataReference? FromType<T>() => FromType(typeof(T));

    public static MetadataReference FromAssembly(Assembly assembly) => FromAssembly(assembly.Location);

    public static MetadataReference FromAssembly(string assemblyLocation) =>
        MetadataReference.CreateFromFile(assemblyLocation);

    public static MetadataReference? FromType(Type type) => MetadataReference.CreateFromFile(type.Assembly.Location);

    public static readonly MetadataReference NetStandard2_0 =
        MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);
}