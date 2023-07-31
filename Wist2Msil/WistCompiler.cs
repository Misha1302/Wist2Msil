namespace Wist2Msil;

using System.Reflection;
using System.Reflection.Emit;
using GrEmit;
using WistConst;

public sealed class WistCompiler
{
    private static readonly Dictionary<string, MethodInfo> _methods = new();
    private static readonly FieldInfo _constsField;
    private readonly WistModule _module;

    static WistCompiler()
    {
        foreach (var m in typeof(WistExecutionHelper).GetMethods(BindingFlags.Public | BindingFlags.Static))
            _methods.Add(m.Name, m);

        _constsField = typeof(WistExecutionHelper).GetField(nameof(WistExecutionHelper.Consts),
                           BindingFlags.Public | BindingFlags.Instance) ??
                       throw new NullReferenceException();
    }

    public WistCompiler(WistModule module)
    {
        _module = module;
    }

    private List<WistExecutionHelper> Compile() => _module.WistFunctions.Select(CompileFunction).ToList();

    private static WistExecutionHelper CompileFunction(WistFunction wistFunction)
    {
        var labels = new Dictionary<string, GroboIL.Label>();

        var consts1 = wistFunction.Image.Instructions.Select(x => x.Constant).ToArray();
        var consts2 = wistFunction.Image.Instructions.Select(x => x.Constant2).ToArray();

        var m = new DynamicMethod(
            wistFunction.Name,
            typeof(WistConst),
            new[] { typeof(WistExecutionHelper) },
            typeof(WistExecutionHelper),
            true
        );

        var executionHelper = new WistExecutionHelper(consts1, m);

        using var il = new GroboIL(m);

        var locals = wistFunction.Image.Locals
            .Select(local => il.DeclareLocal(typeof(WistConst), local, appendUniquePrefix: false))
            .ToDictionary(x => x.Name);

        for (var i = 0; i < wistFunction.Image.Instructions.Count; i++)
        {
            var inst = wistFunction.Image.Instructions[i];
            GroboIL.Label? label;
            switch (inst.Op)
            {
                case WistInstruction.Operation.PushConst:
                    il.Ldarg(0);
                    il.Ldfld(_constsField);
                    il.Ldc_I4(i);
                    il.Ldelem(typeof(WistConst));
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
                    il.Ldfld(_constsField);
                    il.Ldc_I4(i);
                    il.Ldelem(typeof(WistConst));
                    il.Call(_methods[$"Call{consts2[i].GetInternalInteger()}"]);
                    break;
                case WistInstruction.Operation.SetLabel:
                    var name = consts1[i].GetString();
                    if (!labels.TryGetValue(name, out label))
                        labels.Add(name, label = il.DefineLabel(name));

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
                case WistInstruction.Operation.LoadLocal:
                    il.Ldloc(locals[consts1[i].GetString()]);
                    break;
                case WistInstruction.Operation.SetLocal:
                    il.Stloc(locals[consts1[i].GetString()]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        il.Call(_methods["PushDefaultConst"]);
        il.Ret();

        return executionHelper;


        GroboIL.Label AddOrGetLabel(int i)
        {
            var name = consts1[i].GetString();
            if (!labels.TryGetValue(name, out var label))
                labels.Add(name, label = il.DefineLabel(name));
            return label;
        }
    }

    public WistConst Run(out long compilationTime, out long executionTime)
    {
        compilationTime = WistTimer.MeasureExecutionTime(Compile, out var executionHelpers);

        return executionHelpers.First(x => x.DynamicMethod.Name == "Start").Run(out executionTime);
    }
}