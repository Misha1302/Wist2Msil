namespace WistConst;

using System.Reflection.Emit;

public sealed class WistStruct
{
    private readonly FastSortedList<WistConst> _sortedFields = new();
    private readonly FastSortedList<WistMethod> _sortedMethods = new();
    public readonly string Name;

    public WistStruct(string name)
    {
        Name = name;
    }

    public WistConst GetField(int key) => _sortedFields.GetByIndex(_sortedFields.IndexOfKey(key));

    public void SetField(int key, WistConst value) => _sortedFields.SetByIndex(_sortedFields.IndexOfKey(key), value);

    public void AddField(int key, WistConst value) => _sortedFields.Add(key, value);


    public WistConst CallMethod(int key, params object[] args)
    {
        var wistMethod = _sortedMethods.GetByIndex(_sortedMethods.IndexOfKey(key));
        return (WistConst)wistMethod.DynamicMethod.Invoke(null, args.Append(wistMethod.ExecutionHelper).ToArray())!;
    }

    public void AddMethod(int key, DynamicMethod m, WistExecutionHelper executionHelper) =>
        _sortedMethods.Add(key, new WistMethod(m, executionHelper));
}