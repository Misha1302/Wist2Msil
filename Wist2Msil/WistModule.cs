namespace Wist2Msil;

using WistConst;

public sealed class WistModule
{
    private readonly List<WistFunction> _wistFunctions = new();
    private readonly List<WistCompilationStruct> _wistStructs = new();

    public readonly WistHashCode.WistHashCode WistHashCode = new();

    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public IReadOnlyList<WistFunction> WistFunctions => _wistFunctions;

    public IReadOnlyList<WistCompilationStruct> WistStructs => _wistStructs;

    public WistFunction MakeFunction(string name, string[]? strings = null)
    {
        var f = new WistFunction(name, new WistImage(), strings ?? Array.Empty<string>());
        _wistFunctions.Add(f);
        return f;
    }

    public void AddFunction(WistFunction wistFunction)
    {
        _wistFunctions.Add(wistFunction);
    }

    public WistCompilationStruct MakeStruct(string name, string[] strings, string[] methods)
    {
        _wistStructs.Add(new WistCompilationStruct(name, strings, methods));
        return _wistStructs[^1];
    }

    public void AddStruct(WistCompilationStruct wistStruct)
    {
        _wistStructs.Add(wistStruct);
    }
}