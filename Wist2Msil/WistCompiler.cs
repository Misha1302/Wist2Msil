namespace Wist2Msil;

using System.Reflection;
using System.Reflection.Emit;
using GrEmit;
using WistConst;

public sealed class WistCompiler
{
    private static readonly Dictionary<string, MethodInfo> _methods = new();
    private readonly List<WistConst> _consts1;
    private readonly List<WistConst> _consts2;
    private readonly WistExecutionHelper _executionHelper;
    private readonly WistImage _image;
    private Dictionary<string, GroboIL.Label> _labels = new();

    static WistCompiler()
    {
        foreach (var m in typeof(WistExecutionHelper).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            _methods.Add(m.Name, m);
    }

    public WistCompiler(WistModule module)
    {
        _image = module.GetImage();

        _executionHelper = new WistExecutionHelper(_image.Instructions.Select(x => x.Constant));
        _consts2 = _image.Instructions.Select(x => x.Constant2).ToList();
        _consts1 = _image.Instructions.Select(x => x.Constant).ToList();
    }

    private DynamicMethod Compile()
    {
        _labels = new Dictionary<string, GroboIL.Label>();

        var m = new DynamicMethod(
            "WistExecutionMethod",
            typeof(WistConst),
            new[] { typeof(WistExecutionHelper) },
            typeof(WistExecutionHelper),
            true
        );

        using var il = new GroboIL(m);
        for (var i = 0; i < _image.Instructions.Count; i++)
        {
            var inst = _image.Instructions[i];
            GroboIL.Label? label;
            switch (inst.Op)
            {
                case WistInstruction.Operation.PushConst:
                    il.Ldarg(0);
                    il.Ldc_I4(i);
                    il.Call(_methods["PushConst"]);
                    break;
                case WistInstruction.Operation.Add:
                    il.Call(_methods["Add"]);
                    break;
                case WistInstruction.Operation.Sub:
                    il.Call(_methods["Sub"]);
                    break;
                case WistInstruction.Operation.Mul:
                    il.Call(_methods["Mul"]);
                    break;
                case WistInstruction.Operation.Div:
                    il.Call(_methods["Div"]);
                    break;
                case WistInstruction.Operation.Rem:
                    il.Call(_methods["Rem"]);
                    break;
                case WistInstruction.Operation.Pow:
                    il.Call(_methods["Pow"]);
                    break;
                case WistInstruction.Operation.IsEquals:
                    il.Call(_methods["IsEquals"]);
                    break;
                case WistInstruction.Operation.IsNotEquals:
                    il.Call(_methods["IsNotEquals"]);
                    break;
                case WistInstruction.Operation.LessThan:
                    il.Call(_methods["LessThan"]);
                    break;
                case WistInstruction.Operation.GreaterThan:
                    il.Call(_methods["GreaterThan"]);
                    break;
                case WistInstruction.Operation.LessThanOrEquals:
                    il.Call(_methods["LessThanOrEquals"]);
                    break;
                case WistInstruction.Operation.GreaterThanOrEquals:
                    il.Call(_methods["GreaterThanOrEquals"]);
                    break;
                case WistInstruction.Operation.Call:
                    il.Ldarg(0);
                    il.Ldc_I4(i);
                    il.Call(_methods["PushConst"]);
                    
                    il.Call(_methods[$"Call{_consts2[i].GetInternalInteger()}"]);
                    break;
                case WistInstruction.Operation.SetLabel:
                    var name = _consts1[i].GetString();
                    if (!_labels.TryGetValue(name, out label))
                        _labels.Add(name, label = il.DefineLabel(name));

                    il.MarkLabel(label);
                    break;
                case WistInstruction.Operation.Goto:
                    label = AddOrGetLabel(i);

                    il.Br(label);
                    break;
                case WistInstruction.Operation.GotoIfFalse:
                    label = AddOrGetLabel(i);

                    il.Call(_methods["PopBool"]);
                    il.Brfalse(label);
                    break;
                case WistInstruction.Operation.GotoIfTrue:
                    label = AddOrGetLabel(i);

                    il.Call(_methods["PopBool"]);
                    il.Brtrue(label);
                    break;
                case WistInstruction.Operation.Drop:
                    il.Pop();
                    break;
                case WistInstruction.Operation.Dup:
                    il.Dup();
                    break;
                case WistInstruction.Operation.Cmp:
                    il.Call(_methods["Cmp"]);
                    break;
                case WistInstruction.Operation.NegCmp:
                    il.Call(_methods["NegCmp"]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        il.Call(_methods["PushDefaultConst"]);
        il.Ret();

        return m;
        
        
        GroboIL.Label AddOrGetLabel(int i)
        {
            var name = _consts1[i].GetString();
            if (!_labels.TryGetValue(name, out var label))
                _labels.Add(name, label = il.DefineLabel(name));
            return label;
        }
    }

    public unsafe WistConst Run(out long compilationTime, out long executionTime)
    {
        compilationTime = WistTimer.MeasureExecutionTime(Compile, out var dynamicMethod);

        var handle = GetMethodRuntimeHandle(dynamicMethod);
        var fp = handle.GetFunctionPointer();
        executionTime = WistTimer.MeasureExecutionTime(
            () => ((delegate*<WistExecutionHelper, WistConst>)fp)(_executionHelper),
            out var result);

        return result;
    }

    private static RuntimeMethodHandle GetMethodRuntimeHandle(DynamicMethod method)
    {
        var getMethodDescriptorInfo = typeof(DynamicMethod).GetMethod("GetMethodDescriptor",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var handle = (RuntimeMethodHandle)getMethodDescriptorInfo!.Invoke(method, null)!;

        return handle;
    }
}