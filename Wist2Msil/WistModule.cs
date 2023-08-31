namespace Wist2Msil;

using WistConst;
using WistFastList;

public sealed class WistModule
{
    public readonly WistHashCode.WistHashCode HashCode = new();

    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public WistFastList<WistFunction> Functions { get; } = new();

    public WistFastList<WistCompilationStruct> Structs { get; } = new();

    public void AddFunction(WistFunction wistFunction)
    {
        if (Functions.Contains(wistFunction))
            return;

        if (wistFunction is null)
            throw new InvalidOperationException();

        Functions.Add(wistFunction);
    }

    public void AddStruct(WistCompilationStruct wistStruct)
    {
        if (Structs.Contains(wistStruct))
            return;

        if (wistStruct is null)
            throw new InvalidOperationException();

        Structs.Add(wistStruct);
    }
}