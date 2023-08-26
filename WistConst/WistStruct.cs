namespace WistConst;

using System.Reflection.Emit;
using System.Runtime.CompilerServices;

public sealed class WistStruct
{
    private readonly WistFastSortedList<WistConst> _sortedFields = new();
    private readonly WistFastSortedList<WistMethod> _sortedMethods = new();
    private readonly WistFastSortedList<WistExecutionHelper> _executionHelpers;
    public readonly string Name;
    private readonly List<WistStruct> _inheritances;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistStruct(string name, List<WistStruct> inheritances,
        WistFastSortedList<WistExecutionHelper> executionHelpers)
    {
        Name = name;
        _inheritances = inheritances;
        _executionHelpers = executionHelpers;
    }

    private WistStruct(string name, WistFastSortedList<WistConst> sortedFields,
        WistFastSortedList<WistMethod> sortedMethods,
        IEnumerable<WistStruct> inheritances, WistFastSortedList<WistExecutionHelper> executionHelpers)
    {
        Name = name;
        _sortedFields = sortedFields.Copy();
        _sortedMethods = sortedMethods.Copy();
        _inheritances = inheritances.Select(x => x.Copy()).ToList();
        _executionHelpers = executionHelpers;
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
    public WistConst CallMethod(int key, params object?[] args)
    {
        var indexOfKey = _sortedMethods.IndexOfKey(key);

        // call method in current class
        WistMethod? wistMethod = null;
        if (indexOfKey >= 0)
        {
            wistMethod = _sortedMethods.GetByIndex(indexOfKey);
            goto end;
        }

        // call method in parents
        for (var index = _inheritances.Count - 1; index >= 0; index--)
        {
            var inheritance = _inheritances[index];
            indexOfKey = inheritance._sortedMethods.IndexOfKey(key);
            if (indexOfKey < 0) continue;

            wistMethod = inheritance._sortedMethods.GetByIndex(indexOfKey);
            goto end;
        }

        // call extension method
        indexOfKey = _executionHelpers.IndexOfKey(key);
        if (indexOfKey >= 0)
        {
            var helper = _executionHelpers.GetByIndex(indexOfKey);
            args[^1] = helper;
            return (WistConst)helper.DynamicMethod.Invoke(null, args)!;
        }

        if (wistMethod is null)
            return default;

        end:
        args[^1] = wistMethod.ExecutionHelper;
        return (WistConst)wistMethod.DynamicMethod.Invoke(null, args)!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMethod(int key, DynamicMethod m, WistExecutionHelper executionHelper) =>
        _sortedMethods.Add(key, new WistMethod(m, executionHelper));

    public WistStruct Copy() => new(Name, _sortedFields, _sortedMethods, _inheritances, _executionHelpers);
}