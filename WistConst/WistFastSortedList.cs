﻿namespace WistConst;

using System.Runtime.CompilerServices;

public sealed class WistFastSortedList<TValue>
{
    private KeyValuePair<int, TValue>[] _arr = Array.Empty<KeyValuePair<int, TValue>>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistFastSortedList(KeyValuePair<int, TValue>[] arr)
    {
        _arr = new KeyValuePair<int, TValue>[arr.Length];
        Array.Copy(arr, _arr, arr.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistFastSortedList()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ForEach(Action<TValue> act)
    {
        foreach (var el in _arr)
            act(el.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int key, TValue value)
    {
        var binarySearch = BinarySearch(key);
        if (binarySearch >= 0)
            ThrowEntryExists(key);

        var ind = ~binarySearch;

        Array.Resize(ref _arr, _arr.Length + 1); // increase array length by 1
        Array.Copy(_arr, ind, _arr, ind + 1, _arr.Length - ind - 1); // shift elements right from the index
        _arr[ind] = new KeyValuePair<int, TValue>(key, value); // insert element at the index
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowEntryExists(long key)
    {
        throw new ArgumentException($"An entry with the same key already exists. ({key})");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOfKey(int key) => BinarySearch(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TValue GetByIndex(int index) => _arr[index].Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetByIndex(int index, TValue value) =>
        _arr[index] = new KeyValuePair<int, TValue>(_arr[index].Key, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BinarySearch(long key)
    {
        var lo = 0;
        var hi = _arr.Length - 1;
        while (lo <= hi)
        {
            var i = GetMedian(lo, hi);
            switch (Compare(_arr[i].Key, key))
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

    public WistFastSortedList<TValue> Copy() => new(_arr);
}