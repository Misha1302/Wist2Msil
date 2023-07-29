namespace Wist2Msil;

using System.Runtime.CompilerServices;

public sealed class WistStack<T> where T : struct
{
    private readonly T[] _arr;
    private int _sp;

    public WistStack(WistConst.WistConst.Capacity capacity)
    {
        _arr = new T[(int)capacity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(T value) => _arr[_sp++] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Pop() => ref _arr[--_sp];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Drop() => --_sp;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dup()
    {
        _arr[_sp] = _arr[_sp - 1];
        _sp++;
    }
}