namespace Wist2MsilFrontend;

using System.Reflection;

public sealed class WistLibraryManager
{
    private readonly List<MethodInfo> _methods = new();
    private readonly string[] _paths = { @"", @"Content\Code", @"Content", @"Content\Libraries", @"Libraries" };

    public WistLibraryManager()
    {
        AddBuildInFunctions();
    }

    public MethodInfo? GetMethod(string name)
    {
        return _methods.Find(x => CreateName(x) == name);
    }

    private static string CreateName(MethodBase methodInfo) => methodInfo.Name + methodInfo.GetParameters().Length;

    public void AddLibrary(string path, int index = 0)
    {
        var p = Path.GetFullPath(Path.Combine(_paths[index], path));

        var exists = File.Exists(p);
        if (!exists)
            AddLibrary(path, index + 1);
        else AddLibraryInternal(p);
    }

    private void AddLibraryInternal(string path)
    {
        var assembly = Assembly.LoadFile(path);
        var methodInfos = assembly.GetTypes()
            .Where(x => x.GetCustomAttributesData()
                .Any(y => y.AttributeType == typeof(WistLibraryAttribute))
            )
            .SelectMany(x => x.GetMethods()
                .Where(y => y.GetCustomAttributesData()
                    .Any(z => z.AttributeType == typeof(WistFunctionAttribute))
                )
            );

        _methods.AddRange(methodInfos);
    }

    public void AddBuildInFunctions()
    {
        var methodInfos = typeof(WistBuildInFunctions).GetMethods()
            .Where(y => y.GetCustomAttributesData()
                .Any(z => z.AttributeType == typeof(WistFunctionAttribute))
            );

        _methods.AddRange(methodInfos);
    }
}