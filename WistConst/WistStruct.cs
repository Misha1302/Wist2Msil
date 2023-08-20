namespace WistConst;

using System.Reflection.Emit;

public sealed class WistStruct
{
    private readonly SortedList<int, WistConst> _sortedFields = new();
    private readonly SortedList<int, DynamicMethod> _sortedMethods = new();
    public readonly string Name;

    public WistStruct(string name)
    {
        Name = name;
    }

    public WistConst GetField(int key) => _sortedFields[key];
    public void SetField(int key, WistConst value) => _sortedFields[key] = value;


    public DynamicMethod GetMethod(int key) => _sortedMethods[key];
    public void SetMethod(int key, DynamicMethod m) => _sortedMethods[key] = m;
}