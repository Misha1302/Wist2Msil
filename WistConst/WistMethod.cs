namespace WistConst;

using System.Reflection.Emit;
using System.Runtime.CompilerServices;

public sealed class WistMethod
{
    public nint MethodPtr;
    private readonly DynamicMethod _dynamicMethod;
    public readonly WistExecutionHelper ExecutionHelper;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public WistMethod(DynamicMethod dynamicMethod, WistExecutionHelper executionHelper)
    {
        _dynamicMethod = dynamicMethod;
        ExecutionHelper = executionHelper;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Init()
    {
        MethodPtr = WistExecutionHelper.GetMethodRuntimeHandle(_dynamicMethod).GetFunctionPointer();
    }
}