namespace Wist2Msil;

using WistConst;

public sealed class WistModule
{
    private readonly List<WistFunction> _wistFunctions = new();
    private readonly List<WistCompilationStruct> _wistStructs = new();

    public readonly WistHashCode.WistHashCode HashCode = new();

    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public IReadOnlyList<WistFunction> Functions => _wistFunctions;

    public IReadOnlyList<WistCompilationStruct> Structs => _wistStructs;

    public void AddFunction(WistFunction wistFunction)
    {
        if (_wistFunctions.Contains(wistFunction))
            return;

        if (wistFunction is null)
            throw new InvalidOperationException();

        _wistFunctions.Add(wistFunction);
    }

    public void AddStruct(WistCompilationStruct wistStruct)
    {
        if (_wistStructs.Contains(wistStruct))
            return;
        
        if (wistStruct is null)
            throw new InvalidOperationException();

        _wistStructs.Add(wistStruct);
    }
}