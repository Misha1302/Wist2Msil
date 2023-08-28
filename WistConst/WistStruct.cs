namespace WistConst;

using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using WistFastList;

public sealed class WistStruct
{
    private readonly WistFastSortedList<WistConst> _sortedFields = new();
    private readonly WistFastSortedList<WistMethod> _sortedMethods = new();
    private readonly WistFastSortedList<WistExecutionHelper> _executionHelpers;
    public readonly string Name;
    private readonly WistFastList<WistStruct> _inheritances;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistStruct(string name, WistFastList<WistStruct> inheritances,
        WistFastSortedList<WistExecutionHelper> executionHelpers)
    {
        Name = name;
        _inheritances = inheritances;
        _executionHelpers = executionHelpers;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistStruct(string name, WistFastSortedList<WistConst> sortedFields,
        WistFastSortedList<WistMethod> sortedMethods,
        WistFastList<WistStruct> inheritances, WistFastSortedList<WistExecutionHelper> executionHelpers)
    {
        Name = name;
        _sortedFields = sortedFields.Copy();
        _sortedMethods = sortedMethods;
        _executionHelpers = executionHelpers;

        var count = inheritances.Count;
        _inheritances = new WistFastList<WistStruct>(count);
        for (var index = count - 1; index >= 0; index--)
            _inheritances.Add(inheritances[index].Copy());
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
    public unsafe WistConst CallMethod(int key)
    {
        CallMethodInternal(key, out var wistMethod, out var helper);

        if (wistMethod != null)
            return ((delegate*<WistExecutionHelper, WistConst>)wistMethod.MethodPtr)
                (wistMethod.ExecutionHelper);

        if (helper != null)
            return ((delegate*<WistExecutionHelper, WistConst>)helper.MethodPtr)
                (helper);

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe WistConst CallMethod(int key, WistConst a)
    {
        CallMethodInternal(key, out var wistMethod, out var helper);

        if (wistMethod != null)
            return ((delegate*<WistConst, WistExecutionHelper, WistConst>)wistMethod.MethodPtr)
                (a, wistMethod.ExecutionHelper);

        if (helper != null)
            return ((delegate*<WistConst, WistExecutionHelper, WistConst>)helper.MethodPtr)
                (a, helper);

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe WistConst CallMethod(int key, WistConst a, WistConst b)
    {
        CallMethodInternal(key, out var wistMethod, out var helper);

        if (wistMethod != null)
            return ((delegate*<WistConst, WistConst, WistExecutionHelper, WistConst>)wistMethod.MethodPtr)
                (a, b, wistMethod.ExecutionHelper);

        if (helper != null)
            return ((delegate*<WistConst, WistConst, WistExecutionHelper, WistConst>)helper.MethodPtr)
                (a, b, helper);

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe WistConst CallMethod(int key, WistConst a, WistConst b, WistConst c)
    {
        CallMethodInternal(key, out var wistMethod, out var helper);

        if (wistMethod != null)
            return ((delegate*<WistConst, WistConst, WistConst, WistExecutionHelper, WistConst>)wistMethod.MethodPtr)
                (a, b, c, wistMethod.ExecutionHelper);

        if (helper != null)
            return ((delegate*<WistConst, WistConst, WistConst, WistExecutionHelper, WistConst>)helper.MethodPtr)
                (a, b, c, helper);

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe WistConst CallMethod(int key, WistConst a, WistConst b, WistConst c, WistConst d)
    {
        CallMethodInternal(key, out var wistMethod, out var helper);

        if (wistMethod != null)
            return ((delegate*<WistConst, WistConst, WistConst, WistConst, WistExecutionHelper, WistConst>)wistMethod
                    .MethodPtr)
                (a, b, c, d, wistMethod.ExecutionHelper);

        if (helper != null)
            return ((delegate*<WistConst, WistConst, WistConst, WistConst, WistExecutionHelper, WistConst>)helper
                    .MethodPtr)
                (a, b, c, d, helper);

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe WistConst CallMethod(int key, WistConst a, WistConst b, WistConst c, WistConst d, WistConst f)
    {
        CallMethodInternal(key, out var wistMethod, out var helper);

        if (wistMethod != null)
            return ((delegate*<WistConst, WistConst, WistConst, WistConst, WistConst, WistExecutionHelper, WistConst>)
                    wistMethod.MethodPtr)
                (a, b, c, d, f, wistMethod.ExecutionHelper);

        if (helper != null)
            return ((delegate*<WistConst, WistConst, WistConst, WistConst, WistConst, WistExecutionHelper, WistConst>)
                    helper
                        .MethodPtr)
                (a, b, c, d, f, helper);

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe WistConst CallMethod(int key, WistConst a, WistConst b, WistConst c, WistConst d, WistConst f,
        WistConst g)
    {
        CallMethodInternal(key, out var wistMethod, out var helper);

        if (wistMethod != null)
            return ((delegate*<WistConst, WistConst, WistConst, WistConst, WistConst, WistConst, WistExecutionHelper,
                        WistConst>)
                    wistMethod.MethodPtr)
                (a, b, c, d, f, g, wistMethod.ExecutionHelper);

        if (helper != null)
            return ((delegate*<WistConst, WistConst, WistConst, WistConst, WistConst, WistConst, WistExecutionHelper,
                    WistConst>)helper
                    .MethodPtr)
                (a, b, c, d, f, g, helper);

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CallMethodInternal(int key, out WistMethod? wistMethod, out WistExecutionHelper? helper)
    {
        var indexOfKey = _sortedMethods.IndexOfKey(key);

        wistMethod = null;
        helper = null;

        // call method in current class
        if (indexOfKey >= 0)
        {
            wistMethod = _sortedMethods.GetByIndex(indexOfKey);
            return;
        }

        // call method in parents
        for (var index = _inheritances.Count - 1; index >= 0; index--)
        {
            var inheritance = _inheritances[index];
            indexOfKey = inheritance._sortedMethods.IndexOfKey(key);
            if (indexOfKey < 0) continue;

            wistMethod = inheritance._sortedMethods.GetByIndex(indexOfKey);
            return;
        }

        // call extension method
        indexOfKey = _executionHelpers.IndexOfKey(key);
        if (indexOfKey >= 0)
            helper = _executionHelpers.GetByIndex(indexOfKey);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMethod(int key, DynamicMethod m, WistExecutionHelper executionHelper) =>
        _sortedMethods.Add(key, new WistMethod(m, executionHelper));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Init()
    {
        _sortedMethods.ForEach(x => x.Init());
        _executionHelpers.ForEach(x => x.Init());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistStruct Copy() => new(Name, _sortedFields, _sortedMethods, _inheritances, _executionHelpers);
}