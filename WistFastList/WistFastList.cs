namespace WistFastList;

using System.Collections;
using System.Runtime.CompilerServices;

public sealed class WistFastList<T> : IEnumerable<T>
{
    private T[] _arr;
    private int _capacity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistFastList(int capacity = 16)
    {
        _arr = new T[_capacity = capacity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistFastList(T[] arr, int capacity)
    {
        _arr = arr;
        _capacity = capacity;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
    }


    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetByIndex(index);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SetByIndex(index, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_arr[..Count]).GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value)
    {
        TryGrow();

        _arr[Count++] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetByIndex(int ind)
    {
        ThrowOutOfBounds(ind);
        return _arr[ind];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetByIndex(int ind, T value)
    {
        ThrowOutOfBounds(ind);
        _arr[ind] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowOutOfBounds(int ind)
    {
        if (ind < 0 || ind >= Count)
            throw new IndexOutOfRangeException(ind.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveAt(int ind)
    {
        Array.Copy(_arr, ind + 1, _arr, ind, _arr.Length - ind - 1);

        Count--;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TryGrow()
    {
        if (Count + 1 >= _capacity)
            Array.Resize(ref _arr, _capacity = _capacity * 2 + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => string.Join(", ", _arr[..Count]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(T[] arr)
    {
        Array.Resize(ref _arr, Count + arr.Length);
        Array.Copy(arr, 0, _arr, Count, arr.Length);
        Count = _arr.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(IEnumerable<T> collection) => AddRange(collection.ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistFastList<T> Copy() => new(CopyArray(_arr), _capacity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T[] CopyArray(T[] arr)
    {
        var newArr = new T[arr.Length];
        Array.Copy(arr, newArr, arr.Length);

        return newArr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int ind, T elem)
    {
        TryGrow();

        Array.Copy(_arr, ind, _arr, ind + 1, Count - ind - 1); // shift elements right from the index
        _arr[ind] = elem;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Remove(T elem)
    {
        RemoveAt(IndexOf(elem));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOf(T elem) => Array.IndexOf(_arr, elem);
}