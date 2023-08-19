﻿namespace Wist2Msil;

public sealed class WistModule
{
    private readonly List<WistFunction> _wistFunctions = new();
    private readonly List<WistCompilationStruct> _wistStructs = new();

    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public IReadOnlyList<WistFunction> WistFunctions => _wistFunctions;

    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public IReadOnlyList<WistCompilationStruct> WistStructs => _wistStructs;

    public WistFunction MakeFunction(string name, string[]? strings = null)
    {
        var f = new WistFunction(name, new WistImage(), strings ?? Array.Empty<string>());
        _wistFunctions.Add(f);
        return f;
    }

    public WistCompilationStruct MakeStruct(string name, string[] strings, string[] methods)
    {
        _wistStructs.Add(new WistCompilationStruct(name, strings, methods));
        return _wistStructs[^1];
    }
}