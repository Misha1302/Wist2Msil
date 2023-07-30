namespace Wist2Msil;

using System.Runtime.CompilerServices;
using WistConst;

public sealed unsafe class WistExecutionHelper
{
    private readonly WistConst[] _consts;

    public WistExecutionHelper(IEnumerable<WistConst> consts)
    {
        _consts = consts.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Add(WistConst a, WistConst b) => WistConstOperations.Add(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Sub(WistConst a, WistConst b) => WistConstOperations.Sub(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Mul(WistConst a, WistConst b) => WistConstOperations.Mul(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Div(WistConst a, WistConst b) => WistConstOperations.Div(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PopBool(WistConst a) => a.GetBool();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Rem(WistConst a, WistConst b) => WistConstOperations.Rem(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Pow(WistConst a, WistConst b) => WistConstOperations.Pow(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThan(WistConst a, WistConst b) => WistConstOperations.LessThan(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThanOrEquals(WistConst a, WistConst b) =>
        WistConstOperations.LessThanOrEquals(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThan(WistConst a, WistConst b) => WistConstOperations.GreaterThan(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThanOrEquals(WistConst a, WistConst b) =>
        WistConstOperations.GreaterThanOrEquals(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst IsEquals(WistConst a, WistConst b) => WistConstOperations.Equals(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst IsNotEquals(WistConst a, WistConst b) => WistConstOperations.NotEquals(a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Cmp(WistConst a, WistConst b) => new(a == b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst NegCmp(WistConst a, WistConst b) => new(a != b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst PushDefaultConst() => default;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistConst PushConst(int index) => _consts[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Call0(WistConst ptr)
    {
        var pointer = (delegate*<WistConst>)ptr.GetPointer();
        return pointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Call1(WistConst a, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst>)ptr.GetPointer();
        return pointer(a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Call2(WistConst a, WistConst b, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst, WistConst>)ptr.GetPointer();
        return pointer(a, b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Call3(WistConst a, WistConst b, WistConst c, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst, WistConst, WistConst>)ptr.GetPointer();
        return pointer(a, b, c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Call4(WistConst a, WistConst b, WistConst c, WistConst d, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst, WistConst, WistConst, WistConst>)ptr.GetPointer();
        return pointer(a, b, c, d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Call5(WistConst a, WistConst b, WistConst c, WistConst d, WistConst e, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst, WistConst, WistConst, WistConst, WistConst>)ptr.GetPointer();
        return pointer(a, b, c, d, e);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Call6(WistConst a, WistConst b, WistConst c, WistConst d, WistConst e, WistConst f, WistConst ptr)
    {
        var pointer =
            (delegate*<WistConst, WistConst, WistConst, WistConst, WistConst, WistConst, WistConst>)
            ptr.GetPointer();
        return pointer(a, b, c, d, e, f);
    }
}