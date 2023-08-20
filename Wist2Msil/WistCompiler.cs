namespace Wist2Msil;

using System.Reflection;
using System.Reflection.Emit;
using GrEmit;
using Wist2Msil.WistHashCode;
using WistConst;

public sealed class WistCompiler
{
    private static readonly IReadOnlyDictionary<string, MethodInfo> _methods;
    private static readonly FieldInfo _constsField;
    private static readonly FieldInfo _executionHelpersField;
    private readonly WistModule _module;
    private List<WistExecutionHelper> _executionHelpers = null!;

    static WistCompiler()
    {
        _methods = typeof(WistExecutionHelper).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .ToDictionary(m => m.Name);

        _constsField = typeof(WistExecutionHelper).GetField(nameof(WistExecutionHelper.Consts),
                           BindingFlags.Public | BindingFlags.Instance) ??
                       throw new NullReferenceException();

        _executionHelpersField = typeof(WistExecutionHelper).GetField(nameof(WistExecutionHelper.WistExecutionHelpers),
                                     BindingFlags.Public | BindingFlags.Instance) ??
                                 throw new NullReferenceException();
    }

    public WistCompiler(WistModule module)
    {
        _module = module;
    }

    private void Compile()
    {
        _executionHelpers = new List<WistExecutionHelper>();

        foreach (var wistFunction in _module.WistFunctions)
            DeclareFunction(wistFunction);

        foreach (var exeHelper in _executionHelpers)
            exeHelper.WistExecutionHelpers = _executionHelpers.ToArray();

        foreach (var wistFunction in _module.WistFunctions)
            CompileFunction(wistFunction);
    }

    private void DeclareFunction(WistFunction wistFunc)
    {
        var parameterTypes =
            new[] { typeof(WistExecutionHelper) }.Union(wistFunc.Parameters.Select(_ => typeof(WistConst))).ToArray();

        var executionHelper = new WistExecutionHelper(wistFunc.Image.Instructions.Select(x => x.Constant),
            new DynamicMethod(
                wistFunc.Name,
                typeof(WistConst),
                parameterTypes,
                typeof(WistExecutionHelper),
                true
            ),
            Array.Empty<WistExecutionHelper>()
        );

        _executionHelpers.Add(executionHelper);
    }

    private void CompileFunction(WistFunction wistFunc)
    {
        var labels = new Dictionary<string, GroboIL.Label>();

        var consts1 = wistFunc.Image.Instructions.Select(x => x.Constant).ToArray();
        var consts2 = wistFunc.Image.Instructions.Select(x => x.Constant2).ToArray();

        var curExeHelper = _executionHelpers.Find(x => x.DynamicMethod.Name == wistFunc.Name)!;
        using var il = new GroboIL(curExeHelper.DynamicMethod);

        var locals = wistFunc.Image.Locals
            .Select(local => il.DeclareLocal(typeof(WistConst), local, appendUniquePrefix: false))
            .ToDictionary(x => x.Name);

        for (var i = 0; i < wistFunc.Image.Instructions.Count; i++)
        {
            var inst = wistFunc.Image.Instructions[i];
            GroboIL.Label? label;
            int ind;
            switch (inst.Op)
            {
                case WistInstruction.Operation.PushConst:
                    Push(il, i);
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
                case WistInstruction.Operation.CSharpCall:
                    il.Ldarg(0);
                    il.Ldfld(_constsField);
                    il.Ldc_I4(i);
                    il.Ldelem(typeof(WistConst));
                    il.Call(_methods[$"CSharpCall{consts2[i].GetInternalInteger()}"]);
                    break;
                case WistInstruction.Operation.WistCall:
                    ind = _executionHelpers.FindIndex(
                        x => x.DynamicMethod.Name == consts2[i].GetString()
                    );

                    il.Ldarg(0);
                    il.Ldfld(_executionHelpersField);
                    il.Ldc_I4(ind);
                    il.Ldelem(typeof(WistExecutionHelper));
                    il.Call(_executionHelpers[ind].DynamicMethod);
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
                case WistInstruction.Operation.LoadArg:
                    il.Ldarg(Array.IndexOf(wistFunc.Parameters, consts1[i].GetString()));
                    break;
                case WistInstruction.Operation.Ret:
                    il.Ret();
                    break;
                case WistInstruction.Operation.Instantiate:
                    var src = consts1[i].GetStructInternal();
                    var s = new WistStruct(src.Name);
                    foreach (var field in src.Fields)
                        s.AddField(_module.WistHashCode.GetHashCode(field), default);
                    foreach (var method in src.Methods)
                        s.AddMethod(
                            _module.WistHashCode.GetHashCode(method),
                            _executionHelpers.Find(x => x.DynamicMethod.Name == method)!.DynamicMethod
                        );
                    curExeHelper.Consts[i] = new WistConst(s);

                    Push(il, i);
                    break;
                case WistInstruction.Operation.SetField:
                    il.Ldc_I4(consts1[i].GetString().GetWistHashCode(_module));
                    il.Call(_methods["SetField"]);
                    break;
                case WistInstruction.Operation.PushField:
                    il.Ldc_I4(consts1[i].GetString().GetWistHashCode(_module));
                    il.Call(_methods["GetField"]);
                    break;
                case WistInstruction.Operation.CallStructMethod:
                    ind = _executionHelpers.FindIndex(
                        x => x.DynamicMethod.Name == consts1[i].GetString()
                    );

                    il.Ldarg(0);
                    il.Ldfld(_executionHelpersField);
                    il.Ldc_I4(ind);
                    il.Ldelem(typeof(WistExecutionHelper));

                    il.Ldc_I4(consts1[i].GetString().GetWistHashCode(_module));
                    il.Call(_methods[$"CallStructMethod{consts2[i].GetInternalInteger()}"]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(inst.Op.ToString());
            }
        }

        il.Call(_methods["PushDefaultConst"]);
        il.Ret();


        GroboIL.Label AddOrGetLabel(int i)
        {
            var name = consts1[i].GetString();
            if (!labels.TryGetValue(name, out var label))
                labels.Add(name, label = il.DefineLabel(name));
            return label;
        }
    }

    private static void Push(GroboIL il, int i)
    {
        il.Ldarg(0);
        il.Ldfld(_constsField);
        il.Ldc_I4(i);
        il.Ldelem(typeof(WistConst));
    }

    public WistConst Run(out long compilationTime, out long executionTime)
    {
        compilationTime = WistTimer.MeasureExecutionTime(Compile);

        var wistExecutionHelper = _executionHelpers.First(x => x.DynamicMethod.Name == "Start");
        var wistConst = wistExecutionHelper.Run(out executionTime);
        return wistConst;
    }
}