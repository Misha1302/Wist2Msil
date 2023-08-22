namespace WistConst;

using System.Reflection.Emit;

public sealed record WistMethod(DynamicMethod DynamicMethod, WistExecutionHelper ExecutionHelper);