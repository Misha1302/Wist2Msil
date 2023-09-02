namespace WistConst;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WistFastList;

[StructLayout(LayoutKind.Explicit)]
public readonly struct WistConst
{
    [FieldOffset(0)] private readonly nint _valueN;
    [FieldOffset(0)] private readonly double _valueR;
    [FieldOffset(0)] private readonly long _valueL;
    [FieldOffset(0)] private readonly int _valueI;
    [FieldOffset(0)] private readonly bool _valueB;

    [FieldOffset(8)] public readonly WistType Type;

    [FieldOffset(16)] private readonly WistFastList<WistConst> _list;
    [FieldOffset(16)] private readonly string _str;
    [FieldOffset(16)] private readonly WistStruct _struct;
    [FieldOffset(16)] private readonly WistCompilationStruct _wistCompilationStruct;
    [FieldOffset(16)] private readonly MethodInfo _methodInfo;
    [FieldOffset(16)] private readonly WistRepeatEnumerator _repeatEnumerator;

    private static readonly WistConst _null = new(WistType.Null);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void SkipInitAll(out nint valueN, out double valueR, out long valueL, out int valueI,
        out bool valueB,
        out WistType type, out WistFastList<WistConst> list, out string str, out WistStruct @struct,
        out WistCompilationStruct cStruct, out MethodInfo mInfo, out WistRepeatEnumerator repeatEnumerator)
    {
        Unsafe.SkipInit(out valueN);
        Unsafe.SkipInit(out valueR);
        Unsafe.SkipInit(out valueL);
        Unsafe.SkipInit(out valueI);
        Unsafe.SkipInit(out valueB);
        Unsafe.SkipInit(out type);
        Unsafe.SkipInit(out list);
        Unsafe.SkipInit(out str);
        Unsafe.SkipInit(out @struct);
        Unsafe.SkipInit(out cStruct);
        Unsafe.SkipInit(out mInfo);
        Unsafe.SkipInit(out repeatEnumerator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(string v)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _str = v;
        Type = WistType.String;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(double v)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _valueR = v;
        Type = WistType.Number;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateInternalConst(int i) => new(i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(int i)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _valueI = i;
        Type = WistType.InternalInteger;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateInternalConst(nint i) => new(i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(nint ptr)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _valueN = ptr;
        Type = WistType.Pointer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(bool b)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _valueB = b;
        Type = WistType.Bool;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(WistFastList<WistConst> wistConsts)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _list = wistConsts;
        Type = WistType.List;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(WistStruct s)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _struct = s;
        Type = WistType.Struct;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst(WistCompilationStruct mCompilationStruct)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _wistCompilationStruct = mCompilationStruct;
        Type = WistType.StructInternal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private WistConst(WistType type)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        Type = type;
    }

    public WistConst(MethodInfo mInfo)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _methodInfo = mInfo;
        Type = WistType.MInfo;
    }

    public WistConst(WistRepeatEnumerator repeatEnumerator)
    {
        SkipInitAll(out _valueN, out _valueR, out _valueL, out _valueI, out _valueB, out Type, out _list, out _str,
            out _struct, out _wistCompilationStruct, out _methodInfo, out _repeatEnumerator);
        _repeatEnumerator = repeatEnumerator;
        Type = WistType.RepeatEnumerator;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double GetNumber() => _valueR;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInternalInteger() => _valueI;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetBool() => _valueB;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EqualsConsts(in WistConst obj)
    {
        if (((int)obj.Type & (int)WistType.ValueType) != 0)
        {
            if (obj.Type == WistType.Number)
                return Math.Abs(obj._valueR - _valueR) < 0.001;

            return (obj._valueL ^ (byte)obj.Type) == (_valueL ^ (byte)Type);
        }

        return obj.Type switch
        {
            WistType.String => obj.GetString() == GetString(),
            WistType.List => obj.GetWistFastList().SequenceEqual(GetWistFastList()),
            _ => Type == obj.Type
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nint GetPointer() => _valueN;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => WistConstOperations.ToStr(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst CopyStruct() => new(GetWistStruct().Copy());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateList(WistConst[] arr) => new(new WistFastList<WistConst>(arr));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CreateNull() => _null;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistStruct GetWistStruct() => _struct;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistFastList<WistConst> GetWistFastList() => _list;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetString() => _str;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistCompilationStruct GetWistCompStruct() => _wistCompilationStruct;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MethodInfo GetMethodInfo() => _methodInfo;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistRepeatEnumerator GetRepeatEnumerator() => _repeatEnumerator;
}