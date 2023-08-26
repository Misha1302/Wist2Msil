namespace WistConst;

using System.Runtime.CompilerServices;

public sealed class WistFastSortedList<TValue>
{
    private readonly List<KeyValuePair<int, TValue>> _list = new();

    private WistFastSortedList(IEnumerable<KeyValuePair<int, TValue>> list)
    {
        _list = list.ToList();
    }

    public WistFastSortedList()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int key, TValue value)
    {
        var binarySearch = BinarySearch(key);
        if (binarySearch >= 0)
            ThrowEntryExists(key);

        _list.Insert(~binarySearch, new KeyValuePair<int, TValue>(key, value));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowEntryExists(long key)
    {
        throw new ArgumentException($"An entry with the same key already exists. ({key})");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOfKey(int key) => BinarySearch(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TValue GetByIndex(int index) => _list[index].Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetByIndex(int index, TValue value) =>
        _list[index] = new KeyValuePair<int, TValue>(_list[index].Key, value);


    /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int LinerSearch(long key)
    {
        var listCount = _list.Count;
        for (var i = 0; i < listCount; i++)
            if (_list[i].Key == key)
                return i;

        return -1;
    }*/

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BinarySearch(long key)
    {
        var lo = 0;
        var hi = _list.Count - 1;
        while (lo <= hi)
        {
            var i = GetMedian(lo, hi);
            switch (Compare(_list[i].Key, key))
            {
                case < 0:
                    lo = i + 1;
                    break;
                case 0:
                    return i;
                default:
                    hi = i - 1;
                    break;
            }
        }

        return ~lo;
    }

    // return and parameters types must be long, otherwise there may be an overflow
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Compare(long a, long b) => a - b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetMedian(int lo, int hi) => lo + (hi - lo) / 2;

    public WistFastSortedList<TValue> Copy() => new(_list);
}