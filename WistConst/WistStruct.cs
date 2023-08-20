namespace WistConst;

using System.Reflection.Emit;
using Wist2Msil;

public sealed class WistStruct
{
    private readonly FastSortedList<WistConst> _sortedFields = new();
    private readonly FastSortedList<DynamicMethod> _sortedMethods = new();
    public readonly string Name;

    public WistStruct(string name)
    {
        Name = name;
    }

    public WistConst GetField(int key) => _sortedFields.GetByIndex(_sortedFields.IndexOfKey(key));

    public void SetField(int key, WistConst value) => _sortedFields.SetByIndex(_sortedFields.IndexOfKey(key), value);

    public void AddField(int key, WistConst value) => _sortedFields.Add(key, value);


    public DynamicMethod GetMethod(int key) => _sortedMethods.GetByIndex(_sortedMethods.IndexOfKey(key));
    public void SetMethod(int key, DynamicMethod m) => _sortedMethods.SetByIndex(_sortedMethods.IndexOfKey(key), m);
    public void AddMethod(int key, DynamicMethod m) => _sortedMethods.Add(key, m);
}