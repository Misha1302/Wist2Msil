namespace WistConst;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public readonly struct WistConst : IEquatable<WistConst>
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
        Unsafe.SkipInit(out _valueN);
        Unsafe.SkipInit(out _valueL);
        Unsafe.SkipInit(out _valueI);
        Unsafe.SkipInit(out _valueB);
        Unsafe.SkipInit(out _valueR);
        _handle = new WistGcHandleProvider(v);
        Type = WistType.String;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(double v)
    {
        Unsafe.SkipInit(out _valueN);
        Unsafe.SkipInit(out _valueL);
        Unsafe.SkipInit(out _valueI);
        Unsafe.SkipInit(out _valueB);
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
        Unsafe.SkipInit(out _valueN);
        Unsafe.SkipInit(out _valueL);
        Unsafe.SkipInit(out _valueI);
        Unsafe.SkipInit(out _valueR);
        _valueB = b;
        Type = WistType.Bool;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(List<WistConst> wistConsts)
    {
        Unsafe.SkipInit(out _valueN);
        Unsafe.SkipInit(out _valueL);
        Unsafe.SkipInit(out _valueI);
        Unsafe.SkipInit(out _valueB);
        Unsafe.SkipInit(out _valueR);
        _handle = new WistGcHandleProvider(wistConsts);
        Type = WistType.List;
    }

    public WistConst(WistStruct s)
    {
        Unsafe.SkipInit(out _valueN);
        Unsafe.SkipInit(out _valueL);
        Unsafe.SkipInit(out _valueI);
        Unsafe.SkipInit(out _valueB);
        Unsafe.SkipInit(out _valueR);
        _handle = new WistGcHandleProvider(s);
        Type = WistType.Struct;
    }

    public WistConst(WistCompilationStruct mCompilationStruct)
    {
        Unsafe.SkipInit(out _valueN);
        Unsafe.SkipInit(out _valueL);
        Unsafe.SkipInit(out _valueI);
        Unsafe.SkipInit(out _valueB);
        Unsafe.SkipInit(out _valueR);
        _handle = new WistGcHandleProvider(mCompilationStruct);
        Type = WistType.StructInternal;
    }

    private WistConst(WistType type)
    {
        Unsafe.SkipInit(out _valueN);
        Unsafe.SkipInit(out _valueL);
        Unsafe.SkipInit(out _valueI);
        Unsafe.SkipInit(out _valueB);
        Unsafe.SkipInit(out _valueR);
        Unsafe.SkipInit(out _handle);
        Type = type;
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
    public bool Equals(WistConst obj) => EqualsConsts(obj);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is WistConst other && EqualsConsts(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(_valueL, (int)Type, _handle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool EqualsConsts(in WistConst obj) => obj.Type != WistType.String
        ? (obj._valueL ^ (byte)obj.Type) == (_valueL ^ (byte)Type)
        : obj.GetString() == GetString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in WistConst left, in WistConst right) => left.EqualsConsts(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in WistConst left, in WistConst right) => !left.EqualsConsts(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nint GetPointer() => _valueN;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => WistConstOperations.ToStr(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public List<WistConst> GetList() => (List<WistConst>)((GCHandle)_handle!.Pointer).Target!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistStruct GetStruct() => (WistStruct)((GCHandle)_handle!.Pointer).Target!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistCompilationStruct GetStructInternal() => (WistCompilationStruct)((GCHandle)_handle!.Pointer).Target!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateNull() => new(WistType.Null);
}