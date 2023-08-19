namespace Wist2Msil;

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using WistConst;

public sealed unsafe class WistExecutionHelper
{
    public readonly DynamicMethod DynamicMethod;
    public WistExecutionHelper[] WistExecutionHelpers;
    public WistConst[] Consts;

    public WistExecutionHelper(IEnumerable<WistConst> consts, DynamicMethod dynamicMethod,
        WistExecutionHelper[] wistExecutionHelpers)
    {
        Consts = consts.ToArray();
        DynamicMethod = dynamicMethod;
        WistExecutionHelpers = wistExecutionHelpers;
    }

    public WistConst Run(out long executionTime)
    {
        var fp = GetMethodRuntimeHandle(DynamicMethod).GetFunctionPointer();
        executionTime = WistTimer.MeasureExecutionTime(() => ((delegate*<WistExecutionHelper, WistConst>)fp)(this),
            out var result);
        return result;
    }


    public static RuntimeMethodHandle GetMethodRuntimeHandle(DynamicMethod method)
    {
        var getMethodDescriptorInfo = typeof(DynamicMethod).GetMethod("GetMethodDescriptor",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var handle = (RuntimeMethodHandle)getMethodDescriptorInfo!.Invoke(method, null)!;

        return handle;
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
    public static void SetField(WistConst wistStruct, WistConst constValue, int key) =>
        wistStruct.GetStruct().SetField(key, constValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GetField(WistConst wistStruct, int key) =>
        wistStruct.GetStruct().GetField(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CSharpCall0(WistConst ptr)
    {
        var pointer = (delegate*<WistConst>)ptr.GetPointer();
        return pointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CSharpCall1(WistConst a, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst>)ptr.GetPointer();
        return pointer(a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CSharpCall2(WistConst a, WistConst b, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst, WistConst>)ptr.GetPointer();
        return pointer(a, b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CSharpCall3(WistConst a, WistConst b, WistConst c, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst, WistConst, WistConst>)ptr.GetPointer();
        return pointer(a, b, c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CSharpCall4(WistConst a, WistConst b, WistConst c, WistConst d, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst, WistConst, WistConst, WistConst>)ptr.GetPointer();
        return pointer(a, b, c, d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CSharpCall5(WistConst a, WistConst b, WistConst c, WistConst d, WistConst e, WistConst ptr)
    {
        var pointer = (delegate*<WistConst, WistConst, WistConst, WistConst, WistConst, WistConst>)ptr.GetPointer();
        return pointer(a, b, c, d, e);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CSharpCall6(WistConst a, WistConst b, WistConst c, WistConst d, WistConst e, WistConst f,
        WistConst ptr)
    {
        var pointer =
            (delegate*<WistConst, WistConst, WistConst, WistConst, WistConst, WistConst, WistConst>)
            ptr.GetPointer();
        return pointer(a, b, c, d, e, f);
    }
}