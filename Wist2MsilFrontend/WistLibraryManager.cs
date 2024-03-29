﻿namespace Wist2MsilFrontend;

using System.Reflection;
using WistFastList;

public sealed class WistLibraryManager
{
    private readonly WistFastList<MethodInfo> _methods = new();

    private readonly string[] _paths =
        { @"", @"Content\Code", @"Content\Code\Libraries", @"Content", @"Content\Libraries", @"Libraries" };

    public MethodInfo? GetMethod(string name)
    {
        return _methods.FirstOrDefault(x => CreateName(x) == name);
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
                    .Any(z => z.AttributeType == typeof(WistLibraryFunctionAttribute))
                )
            ).ToArray();

        _methods.AddRange(methodInfos);
    }
}