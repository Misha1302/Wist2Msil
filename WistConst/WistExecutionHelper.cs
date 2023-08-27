namespace WistConst;

using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using WistTimer;

[DebuggerDisplay("{DynamicMethod.Name}")]
public sealed unsafe class WistExecutionHelper
{
    public readonly DynamicMethod DynamicMethod;
    public WistExecutionHelper[] ExecutionHelpers;
    public WistConst[] Consts;

    public WistExecutionHelper(IEnumerable<WistConst> consts, DynamicMethod dynamicMethod,
        WistExecutionHelper[] wistExecutionHelpers)
    {
        Consts = consts.ToArray();
        DynamicMethod = dynamicMethod;
        ExecutionHelpers = wistExecutionHelpers;
    }

    public nint MethodPtr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private set;
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
        var handle = (RuntimeMethodHandle)getMethodDescriptorInfo!.Invoke(method, Array.Empty<object?>())!;

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
    public static WistConst Cmp(WistConst a, WistConst b) => new(a == b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst NegCmp(WistConst a, WistConst b) => new(a != b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst PushNullConst() => WistConst.CreateNull();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetField(WistConst wistStruct, WistConst constValue, int key) =>
        wistStruct.Get<WistStruct>().SetField(key, constValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GetField(WistConst wistStruct, int key) =>
        wistStruct.Get<WistStruct>().GetField(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CallStructMethod0(WistConst wistStruct, int key) =>
        wistStruct.Get<WistStruct>().CallMethod(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CallStructMethod1(WistConst wistStruct, WistConst a, int key) =>
        wistStruct.Get<WistStruct>().CallMethod(key, a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CallStructMethod2(WistConst wistStruct, WistConst a, WistConst b, int key) =>
        wistStruct.Get<WistStruct>().CallMethod(key, a, b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CallStructMethod3(WistConst wistStruct, WistConst a, WistConst b, WistConst c, int key) =>
        wistStruct.Get<WistStruct>().CallMethod(key, a, b, c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CallStructMethod4(WistConst wistStruct, WistConst a, WistConst b, WistConst c,
        WistConst d, int key) =>
        wistStruct.Get<WistStruct>().CallMethod(key, a, b, c, d);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CallStructMethod5(WistConst wistStruct, WistConst a, WistConst b, WistConst c,
        WistConst d, WistConst e, int key) =>
        wistStruct.Get<WistStruct>().CallMethod(key, a, b, c, d, e);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst CallStructMethod6(WistConst wistStruct, WistConst a, WistConst b, WistConst c,
        WistConst d, WistConst e, WistConst f, int key) =>
        wistStruct.Get<WistStruct>().CallMethod(key, a, b, c, d, e, f);

    public void Init()
    {
        MethodPtr = GetMethodRuntimeHandle(DynamicMethod).GetFunctionPointer();
    }
}