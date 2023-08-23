namespace WistConst;

using System.Reflection.Emit;
using System.Runtime.CompilerServices;

public sealed class WistStruct
{
    private readonly WistFastSortedList<WistConst> _sortedFields = new();
    private readonly WistFastSortedList<WistMethod?> _sortedMethods = new();
    public readonly string Name;
    private readonly List<WistStruct> _inheritances;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistStruct(string name, List<WistStruct> inheritances)
    {
        Name = name;
        _inheritances = inheritances;
    }

    private WistStruct(string name, WistFastSortedList<WistConst> sortedFields,
        WistFastSortedList<WistMethod?> sortedMethods,
        List<WistStruct> inheritances)
    {
        Name = name;
        _sortedFields = sortedFields.Copy();
        _sortedMethods = sortedMethods.Copy();
        _inheritances = inheritances.Select(x => x.Copy()).ToList();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddInheritance(WistStruct wistStruct)
    {
        _inheritances.Add(wistStruct);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst GetField(int key)
    {
        var indexOfKey = _sortedFields.IndexOfKey(key);
        if (indexOfKey >= 0)
            return _sortedFields.GetByIndex(indexOfKey);

        for (var index = _inheritances.Count - 1; index >= 0; index--)
        {
            var inheritance = _inheritances[index];
            indexOfKey = inheritance._sortedFields.IndexOfKey(key);
            if (indexOfKey >= 0)
                return inheritance._sortedFields.GetByIndex(indexOfKey);
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetField(int key, WistConst value)
    {
        var indexOfKey = _sortedFields.IndexOfKey(key);
        if (indexOfKey >= 0)
        {
            _sortedFields.SetByIndex(indexOfKey, value);
            return;
        }

        for (var index = _inheritances.Count - 1; index >= 0; index--)
        {
            var inheritance = _inheritances[index];
            indexOfKey = inheritance._sortedFields.IndexOfKey(key);
            if (indexOfKey < 0) continue;

            inheritance._sortedFields.SetByIndex(indexOfKey, value);
            return;
        }

        AddField(key, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddField(int key, WistConst value) => _sortedFields.Add(key, value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst CallMethod(int key, params object[] args)
    {
        var indexOfKey = _sortedMethods.IndexOfKey(key);

        WistMethod? wistMethod = null;
        if (indexOfKey >= 0)
        {
            wistMethod = _sortedMethods.GetByIndex(indexOfKey);
            goto end;
        }

        for (var index = _inheritances.Count - 1; index >= 0; index--)
        {
            var inheritance = _inheritances[index];
            indexOfKey = inheritance._sortedMethods.IndexOfKey(key);
            if (indexOfKey < 0) continue;

            wistMethod = inheritance._sortedMethods.GetByIndex(indexOfKey);
            goto end;
        }

        if (wistMethod is null)
            return default;

        end:
        return (WistConst)wistMethod!.DynamicMethod.Invoke(null, args.Append(wistMethod.ExecutionHelper).ToArray())!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMethod(int key, DynamicMethod m, WistExecutionHelper executionHelper) =>
        _sortedMethods.Add(key, new WistMethod(m, executionHelper));

    public WistStruct Copy() => new(Name, _sortedFields, _sortedMethods, _inheritances);
}