namespace WistConst;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public readonly struct WistConst
{
    [FieldOffset(0)] private readonly nint _valueN;
    [FieldOffset(0)] private readonly double _valueR;
    [FieldOffset(0)] private readonly long _valueL;
    [FieldOffset(0)] private readonly int _valueI;
    [FieldOffset(0)] private readonly bool _valueB;

    [FieldOffset(8)] public readonly WistType Type;

    [FieldOffset(16)] private readonly WistGcHandleProvider? _handle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(string v)
    {
        _handle = new WistGcHandleProvider(v);
        Type = WistType.String;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(double v)
    {
        _valueR = v;
        Type = WistType.Number;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateInternalConst(int i) => new(i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(int i)
    {
        _valueI = i;
        Type = WistType.InternalInteger;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateInternalConst(nint i) => new(i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(nint ptr)
    {
        _valueN = ptr;
        Type = WistType.Pointer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(bool b)
    {
        _valueB = b;
        Type = WistType.Bool;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(List<WistConst> wistConsts)
    {
        _handle = new WistGcHandleProvider(wistConsts);
        Type = WistType.List;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetNumber() => _valueR;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInternalInteger() => _valueI;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBool() => _valueB;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetString() => (string)((GCHandle)_handle!.Pointer).Target!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is WistConst c && c.GetHashCode() == GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EqualsConsts(in WistConst obj) => (obj._valueL ^ (long)obj.Type) == ((long)Type ^ _valueL);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => unchecked((int)_valueL ^ (int)(_valueL >> 32));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in WistConst left, in WistConst right) => left.EqualsConsts(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in WistConst left, in WistConst right) => !left.EqualsConsts(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nint GetPointer() => _valueN;

    public override string ToString() => WistConstOperations.ToStr(this);

    public List<WistConst> GetList() => (List<WistConst>)((GCHandle)_handle!.Pointer).Target!;

    public enum Capacity
    {
        C1 = 0b1,
        C3 = 0b11,
        C7 = 0b111,
        C15 = 0b1111,
        C31 = 0b11111,
        C63 = 0b111111,
        C127 = 0b1111111,
        C255 = 0b11111111,
        C511 = 0b111111111,
        C1023 = 0b1111111111
    }
}