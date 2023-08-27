namespace Wist2Msil;

using System.Reflection;
using System.Reflection.Emit;
using GrEmit;
using Wist2Msil.WistHashCode;
using WistConst;
using WistTimer;

public sealed class WistCompiler
{
    private static readonly IReadOnlyDictionary<string, MethodInfo> _methods;
    private static readonly FieldInfo _constsField;
    private static readonly FieldInfo _executionHelpersField;
    private static readonly MethodInfo _copyWistStructMethod;
    private static readonly MethodInfo _copyListMethod;
    private readonly WistModule _module;
    private List<WistExecutionHelper> _executionHelpers = null!;
    private WistFastSortedList<WistExecutionHelper> _sortedListOfHelpers = null!;
    private readonly List<WistStruct> _wistStructures = new();

    static WistCompiler()
    {
        _methods = typeof(WistExecutionHelper).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .ToDictionary(m => m.Name);

        _constsField = typeof(WistExecutionHelper).GetField(nameof(WistExecutionHelper.Consts),
                           BindingFlags.Public | BindingFlags.Instance) ??
                       throw new NullReferenceException();

        _executionHelpersField = typeof(WistExecutionHelper).GetField(nameof(WistExecutionHelper.ExecutionHelpers),
                                     BindingFlags.Public | BindingFlags.Instance) ??
                                 throw new NullReferenceException();

        _copyWistStructMethod =
            typeof(WistConst).GetMethod(nameof(WistConst.CopyStruct), BindingFlags.Instance | BindingFlags.Public) ??
            throw new NullReferenceException();

        _copyListMethod =
            typeof(WistConst).GetMethod(nameof(WistConst.CopyList), BindingFlags.Instance | BindingFlags.Public) ??
            throw new NullReferenceException();
    }

    public WistCompiler(WistModule module)
    {
        _module = module;
    }

    private void Compile()
    {
        _executionHelpers = new List<WistExecutionHelper>();
        _sortedListOfHelpers = new WistFastSortedList<WistExecutionHelper>();

        foreach (var wistFunction in _module.Functions)
            DeclareFunction(wistFunction);

        foreach (var exeHelper in _executionHelpers)
            exeHelper.ExecutionHelpers = _executionHelpers.ToArray();

        foreach (var wistStruct in _module.Structs)
            DeclareStruct(wistStruct);

        foreach (var wistStruct in _module.Structs)
            InitStruct(wistStruct);

        foreach (var wistFunction in _module.Functions)
            CompileFunction(wistFunction);

        foreach (var helper in _executionHelpers)
            _sortedListOfHelpers.Add(_module.HashCode.GetHashCode(helper.DynamicMethod.Name), helper);

        foreach (var s in _module.Structs)
            _wistStructures.Find(x => x.Name == s.Name)!.Init();
    }

    private void InitStruct(WistCompilationStruct wistStruct)
    {
        var src = wistStruct;
        var s = _wistStructures.Find(x => x.Name == src.Name)!;

        foreach (var field in src.Fields)
            s.AddField(_module.HashCode.GetHashCode(field), default);

        foreach (var method in src.Methods)
        {
            var wistExecutionHelper = _executionHelpers.Find(x => x.DynamicMethod.Name == method.FullName);
            s.AddMethod(
                _module.HashCode.GetHashCode(method.NameWithoutOwner),
                wistExecutionHelper!.DynamicMethod,
                wistExecutionHelper
            );
        }

        foreach (var inheritance in src.Inheritances)
            s.AddInheritance(_wistStructures.Find(x => x.Name == inheritance)!);
    }

    private void DeclareStruct(WistCompilationStruct wistStruct)
    {
        _wistStructures.Add(new WistStruct(wistStruct.Name, new List<WistStruct>(), _sortedListOfHelpers));
    }

    private void DeclareFunction(WistFunction wistFunc)
    {
        var parameterTypes = wistFunc.Parameters.Select(_ => typeof(WistConst)).Append(typeof(WistExecutionHelper))
            .ToArray();

        var executionHelper = new WistExecutionHelper(wistFunc.Image.Instructions.Select(x => x.Constant),
            new DynamicMethod(
                wistFunc.Name.FullName,
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
        var exeHelperArgIndex = wistFunc.Parameters.Length;
        var labels = new Dictionary<string, GroboIL.Label>();

        var consts1 = wistFunc.Image.Instructions.Select(x => x.Constant).ToArray();
        var consts2 = wistFunc.Image.Instructions.Select(x => x.Constant2).ToArray();

        var curExeHelper = _executionHelpers.Find(x => x.DynamicMethod.Name == wistFunc.Name.FullName)!;
        using var il = new GroboIL(curExeHelper.DynamicMethod);

        var locals = wistFunc.Image.Locals
            .Select(local => il.DeclareLocal(typeof(WistConst), local, appendUniquePrefix: false))
            .ToDictionary(x => x.Name);

        for (var i = 0; i < wistFunc.Image.Instructions.Count; i++)
        {
            var inst = wistFunc.Image.Instructions[i];
            GroboIL.Label? label;
            string? name;
            switch (inst.Op)
            {
                case WistInstruction.WistOperation.PushConst:
                    Push(il, i, exeHelperArgIndex);
                    break;
                case WistInstruction.WistOperation.Add:
                    il.Call(_methods["Add"]);
                    break;
                case WistInstruction.WistOperation.Sub:
                    il.Call(_methods["Sub"]);
                    break;
                case WistInstruction.WistOperation.Mul:
                    il.Call(_methods["Mul"]);
                    break;
                case WistInstruction.WistOperation.Div:
                    il.Call(_methods["Div"]);
                    break;
                case WistInstruction.WistOperation.Rem:
                    il.Call(_methods["Rem"]);
                    break;
                case WistInstruction.WistOperation.Pow:
                    il.Call(_methods["Pow"]);
                    break;
                case WistInstruction.WistOperation.LessThan:
                    il.Call(_methods["LessThan"]);
                    break;
                case WistInstruction.WistOperation.GreaterThan:
                    il.Call(_methods["GreaterThan"]);
                    break;
                case WistInstruction.WistOperation.LessThanOrEquals:
                    il.Call(_methods["LessThanOrEquals"]);
                    break;
                case WistInstruction.WistOperation.GreaterThanOrEquals:
                    il.Call(_methods["GreaterThanOrEquals"]);
                    break;
                case WistInstruction.WistOperation.CSharpCall:
                    il.Call(consts1[i].Get<MethodInfo>());
                    break;
                case WistInstruction.WistOperation.Call:
                    var ind = _executionHelpers.FindIndex(
                        x => x.DynamicMethod.Name == consts2[i].Get<string>()
                    );

                    il.Ldarg(exeHelperArgIndex);
                    il.Ldfld(_executionHelpersField);
                    il.Ldc_I4(ind);
                    il.Ldelem(typeof(WistExecutionHelper));
                    il.Call(_executionHelpers[ind].DynamicMethod);
                    break;
                case WistInstruction.WistOperation.SetLabel:
                    name = consts1[i].Get<string>();
                    if (!labels.TryGetValue(name, out label))
                        labels.Add(name, label = il.DefineLabel(name));

                    il.MarkLabel(label);
                    break;
                case WistInstruction.WistOperation.Goto:
                    label = AddOrGetLabel(i);

                    il.Br(label);
                    break;
                case WistInstruction.WistOperation.GotoIfFalse:
                    label = AddOrGetLabel(i);

                    il.Call(_methods["PopBool"]);
                    il.Brfalse(label);
                    break;
                case WistInstruction.WistOperation.GotoIfTrue:
                    label = AddOrGetLabel(i);

                    il.Call(_methods["PopBool"]);
                    il.Brtrue(label);
                    break;
                case WistInstruction.WistOperation.Drop:
                    il.Pop();
                    break;
                case WistInstruction.WistOperation.Dup:
                    il.Dup();
                    break;
                case WistInstruction.WistOperation.Cmp:
                    il.Call(_methods["Cmp"]);
                    break;
                case WistInstruction.WistOperation.NegCmp:
                    il.Call(_methods["NegCmp"]);
                    break;
                case WistInstruction.WistOperation.LoadLocal:
                    var argLocalName = consts1[i].Get<string>();

                    if (locals.Any(x => x.Key == argLocalName))
                        il.Ldloc(locals[argLocalName]);
                    else if (wistFunc.Parameters.Any(x => x == argLocalName))
                        il.Ldarg(Array.IndexOf(wistFunc.Parameters, consts1[i].Get<string>()));
                    else throw new InvalidOperationException();

                    break;
                case WistInstruction.WistOperation.SetLocal:
                    name = consts1[i].Get<string>();

                    if (locals.Any(x => x.Key == name))
                        il.Stloc(locals[name]);
                    else if (wistFunc.Parameters.Any(x => x == name))
                        il.Starg(Array.IndexOf(wistFunc.Parameters, consts1[i].Get<string>()));
                    else throw new InvalidOperationException();

                    break;
                case WistInstruction.WistOperation.Ret:
                    il.Ret();
                    break;
                case WistInstruction.WistOperation.Instantiate:
                    var src = consts1[i].Get<WistCompilationStruct>();
                    var s = _wistStructures.Find(x => x.Name == src.Name)!;

                    if (s is null)
                        throw new InvalidOperationException();

                    curExeHelper.Consts[i] = new WistConst(s);

                    il.Ldarg(exeHelperArgIndex);
                    il.Ldfld(_constsField);
                    il.Ldc_I4(i);
                    il.Ldelema(typeof(WistConst));

                    il.Call(_copyWistStructMethod);
                    break;
                case WistInstruction.WistOperation.SetField:
                    il.Ldc_I4(consts1[i].Get<string>().GetWistHashCode(_module));
                    il.Call(_methods["SetField"]);
                    break;
                case WistInstruction.WistOperation.PushField:
                    il.Ldc_I4(consts1[i].Get<string>().GetWistHashCode(_module));
                    il.Call(_methods["GetField"]);
                    break;
                case WistInstruction.WistOperation.CallStructMethod:
                    il.Ldc_I4(consts1[i].Get<string>().GetWistHashCode(_module));
                    il.Call(_methods[$"CallStructMethod{consts2[i].GetInternalInteger() + 1}"]);
                    break;
                case WistInstruction.WistOperation.InstantiateList:
                    curExeHelper.Consts[i] = new WistConst(new List<WistConst>(10));

                    il.Ldarg(exeHelperArgIndex);
                    il.Ldfld(_constsField);
                    il.Ldc_I4(i);
                    il.Ldelema(typeof(WistConst));

                    il.Call(_copyListMethod);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(inst.Op.ToString());
            }
        }

        il.Call(_methods["PushNullConst"]);
        il.Ret();


        GroboIL.Label AddOrGetLabel(int i)
        {
            var name = consts1[i].Get<string>();
            if (!labels.TryGetValue(name, out var label))
                labels.Add(name, label = il.DefineLabel(name));
            return label;
        }
    }

    private static void Push(GroboIL il, int i, int exeHelperArgIndex)
    {
        il.Ldarg(exeHelperArgIndex);
        il.Ldfld(_constsField);
        il.Ldc_I4(i);
        il.Ldelem(typeof(WistConst));
    }

    public WistConst Run(out long compilationTime, out long executionTime)
    {
        compilationTime = WistTimer.MeasureExecutionTime(Compile);

        var wistExecutionHelper = _executionHelpers.First(x => x.DynamicMethod.Name == "Start0");
        var wistConst = wistExecutionHelper.Run(out executionTime);
        return wistConst;
    }
}