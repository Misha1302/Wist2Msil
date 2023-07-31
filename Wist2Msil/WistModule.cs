namespace Wist2Msil;

public sealed class WistModule
{
    private readonly List<WistFunction> _wistFunctions = new();
    public IReadOnlyList<WistFunction> WistFunctions => _wistFunctions;

    public WistFunction MakeFunction(string name)
    {
        var f = new WistFunction(name, new WistImage());
        _wistFunctions.Add(f);
        return f;
    }
}