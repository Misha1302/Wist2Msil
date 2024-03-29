namespace Wist2Msil;

using System.Reflection;
using System.Reflection.Emit;
using GrEmit;
using Wist2Msil.WistHashCode;
using WistConst;
using WistFastList;
using WistTimer;

public sealed class WistCompiler
{
    private static readonly IReadOnlyDictionary<string, MethodInfo> _methods;
    private static readonly FieldInfo _constsField;
    private static readonly FieldInfo _executionHelpersField;
    private static readonly MethodInfo _copyWistStructMethod;
    private static readonly MethodInfo _createListMethod;
    private static readonly MethodInfo _getRepeatEnumerator;
    private static readonly MethodInfo _repeatEnumeratorNext;
    private static readonly MethodInfo _repeatEnumeratorCurrent;
    private static readonly MethodInfo _getExeHelper;
    private static readonly MethodInfo _getMethodPtr;
    private readonly WistModule _module;
    private WistFastList<WistExecutionHelper> _executionHelpers = null!;
    private WistFastSortedList<WistExecutionHelper> _sortedListOfHelpers = null!;
    private readonly WistFastList<WistStruct> _wistStructures = new();

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

        _getRepeatEnumerator =
            typeof(WistConst).GetMethod(nameof(WistConst.GetRepeatEnumerator),
                BindingFlags.Instance | BindingFlags.Public) ??
            throw new NullReferenceException();

        _repeatEnumeratorNext =
            typeof(WistRepeatEnumerator).GetMethod(nameof(WistRepeatEnumerator.Next),
                BindingFlags.Instance | BindingFlags.Public) ??
            throw new NullReferenceException();

        _repeatEnumeratorCurrent =
            typeof(WistRepeatEnumerator).GetMethod(nameof(WistRepeatEnumerator.Current),
                BindingFlags.Instance | BindingFlags.Public) ??
            throw new NullReferenceException();

        _createListMethod =
            typeof(WistConst).GetMethod(nameof(WistConst.CreateList), BindingFlags.Static | BindingFlags.Public) ??
            throw new NullReferenceException();

        _getExeHelper =
            typeof(WistConst).GetMethod(nameof(WistConst.GetExeHelper), BindingFlags.Instance | BindingFlags.Public) ??
            throw new NullReferenceException();

        _getMethodPtr =
            typeof(WistExecutionHelper).GetMethod(nameof(WistExecutionHelper.GetMethodPtr),
                BindingFlags.Instance | BindingFlags.Public) ??
            throw new NullReferenceException();
    }

    public WistCompiler(WistModule module)
    {
        _module = module;
    }

    private void Compile()
    {
        _executionHelpers = new WistFastList<WistExecutionHelper>();
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
            _wistStructures.FirstOrDefault(x => x.Name == s.Name)!.Init();
    }

    private void InitStruct(WistCompilationStruct wistStruct)
    {
        var src = wistStruct;
        var s = _wistStructures.FirstOrDefault(x => x.Name == src.Name)!;

        foreach (var field in src.Fields)
            s.AddField(_module.HashCode.GetHashCode(field), default);

        foreach (var method in src.Methods)
        {
            var wistExecutionHelper = _executionHelpers.FirstOrDefault(x => x.DynamicMethod.Name == method.FullName);
            s.AddMethod(
                _module.HashCode.GetHashCode(method.NameWithoutOwner),
                wistExecutionHelper!.DynamicMethod,
                wistExecutionHelper
            );
        }

        foreach (var inheritance in src.Inheritances)
            s.AddInheritance(_wistStructures.FirstOrDefault(x => x.Name == inheritance)!);
    }

    private void DeclareStruct(WistCompilationStruct wistStruct)
    {
        _wistStructures.Add(new WistStruct(wistStruct.Name, new WistFastList<WistStruct>(), _sortedListOfHelpers));
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

        var curExeHelper = _executionHelpers.FirstOrDefault(x => x.DynamicMethod.Name == wistFunc.Name.FullName)!;
        using var il = new GroboIL(curExeHelper.DynamicMethod);

        var locals = wistFunc.Image.Locals
            .Select(local => il.DeclareLocal(typeof(WistConst), local, appendUniquePrefix: false))
            .ToDictionary(x => x.Name);

        for (var i = 0; i < wistFunc.Image.Instructions.Count; i++)
        {
            var inst = wistFunc.Image.Instructions[i];
#if RELEASE
            try
            {
#endif
            HandleOneOp(wistFunc, inst, il, i, exeHelperArgIndex, consts1, labels, locals, curExeHelper, consts2);
#if RELEASE
            }
            catch
            {
                throw new WistCompilerError(inst.Line, wistFunc.Name);
            }

#endif
        }

        il.Call(_methods["PushNullConst"]);
        il.Ret();
    }

    private void HandleOneOp(WistFunction wistFunc, WistInstruction inst, GroboIL il, int i1, int exeHelperArgIndex,
        WistConst[] consts1, Dictionary<string, GroboIL.Label> labels, Dictionary<string, GroboIL.Local> locals,
        WistExecutionHelper curExeHelper, WistConst[] consts2)
    {
        GroboIL.Label AddOrGetLabel(int i)
        {
            var name = consts1[i].GetString();
            if (!labels.TryGetValue(name, out var label))
                labels.Add(name, label = il.DefineLabel(name));
            return label;
        }

        {
            GroboIL.Label? label;
            string? name;
            switch (inst.Op)
            {
                case WistInstruction.WistOperation.PushConst:
                    Push(il, i1, exeHelperArgIndex);
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
                    il.Call(consts1[i1].GetMethodInfo());
                    break;
                case WistInstruction.WistOperation.Call:
                    var ind = _executionHelpers.ToList().FindIndex(
                        x => x.DynamicMethod.Name == consts1[i1].GetString()
                    );

                    il.Ldarg(exeHelperArgIndex);
                    il.Ldfld(_executionHelpersField);
                    il.Ldc_I4(ind);
                    il.Ldelem(typeof(WistExecutionHelper));
                    il.Call(_executionHelpers[ind].DynamicMethod);
                    break;
                case WistInstruction.WistOperation.SetLabel:
                    name = consts1[i1].GetString();
                    if (!labels.TryGetValue(name, out label))
                        labels.Add(name, label = il.DefineLabel(name));

                    il.MarkLabel(label);
                    break;
                case WistInstruction.WistOperation.Goto:
                    label = AddOrGetLabel(i1);

                    il.Br(label);
                    break;
                case WistInstruction.WistOperation.GotoIfFalse:
                    label = AddOrGetLabel(i1);

                    il.Call(_methods["PopBool"]);
                    il.Brfalse(label);
                    break;
                case WistInstruction.WistOperation.GotoIfTrue:
                    label = AddOrGetLabel(i1);

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
                    var argLocalName = consts1[i1].GetString();

                    if (locals.Any(x => x.Key == argLocalName))
                        il.Ldloc(locals[argLocalName]);
                    else if (wistFunc.Parameters.Any(x => x == argLocalName))
                        il.Ldarg(Array.IndexOf(wistFunc.Parameters, consts1[i1].GetString()));
                    else throw new WistCompilerError(inst.Line, wistFunc.Name);

                    break;
                case WistInstruction.WistOperation.SetLocal:
                    name = consts1[i1].GetString();

                    if (locals.Any(x => x.Key == name))
                        il.Stloc(locals[name]);
                    else if (wistFunc.Parameters.Any(x => x == name))
                        il.Starg(Array.IndexOf(wistFunc.Parameters, consts1[i1].GetString()));
                    else throw new WistCompilerError(inst.Line, wistFunc.Name);

                    break;
                case WistInstruction.WistOperation.Ret:
                    il.Ret();
                    break;
                case WistInstruction.WistOperation.Instantiate:
                    var src = consts1[i1].GetWistCompStruct();
                    var s = _wistStructures.FirstOrDefault(x => x.Name == src.Name)!;

                    if (s is null)
                        throw new WistCompilerError(inst.Line, wistFunc.Name);

                    curExeHelper.Consts[i1] = new WistConst(s);

                    il.Ldarg(exeHelperArgIndex);
                    il.Ldfld(_constsField);
                    il.Ldc_I4(i1);
                    il.Ldelema(typeof(WistConst));

                    il.Call(_copyWistStructMethod);
                    break;
                case WistInstruction.WistOperation.SetField:
                    il.Ldc_I4(consts1[i1].GetString().GetWistHashCode(_module));
                    il.Call(_methods["SetField"]);
                    break;
                case WistInstruction.WistOperation.PushField:
                    il.Ldc_I4(consts1[i1].GetString().GetWistHashCode(_module));
                    il.Call(_methods["GetField"]);
                    break;
                case WistInstruction.WistOperation.CallStructMethod:
                    il.Ldc_I4(consts1[i1].GetString().GetWistHashCode(_module));
                    il.Call(_methods[$"CallStructMethod{consts2[i1].GetInternalInteger() + 1}"]);
                    break;
                case WistInstruction.WistOperation.InstantiateList:
                    var len = (int)(consts1[i1].GetNumber() + 0.1);

                    il.Ldc_I4(len);
                    il.Newarr(typeof(WistConst));
                    var arr = il.DeclareLocal(typeof(WistConst[]));
                    il.Stloc(arr);

                    il.MarkLabel(il.DefineLabel("start_of_array_filling"));
                    var value = il.DeclareLocal(typeof(WistConst));
                    for (var j = 0; j < len; j++)
                    {
                        il.Stloc(value);

                        il.Ldloc(arr);
                        il.Ldc_I4(j);
                        il.Ldelema(typeof(WistConst));
                        il.Ldloc(value);
                        il.Stobj(typeof(WistConst));
                    }

                    il.Ldloc(arr);
                    il.Call(_createListMethod);

                    break;
                case WistInstruction.WistOperation.GotoIfNext:
                    Ldref(il);
                    il.Call(_getRepeatEnumerator);
                    il.Call(_repeatEnumeratorNext);
                    il.Brfalse(AddOrGetLabel(i1));
                    break;
                case WistInstruction.WistOperation.Current:
                    Ldref(il);
                    il.Call(_getRepeatEnumerator);
                    il.Call(_repeatEnumeratorCurrent);
                    break;
                case WistInstruction.WistOperation.InstantiateFunctionPtr:
                    var exeHelper = _executionHelpers.FirstOrDefault(
                        x => x.DynamicMethod.Name == consts1[i1].GetString()
                    );

                    curExeHelper.Consts[i1] = new WistConst(exeHelper ?? throw new InvalidOperationException());
                    Push(il, i1, exeHelperArgIndex);
                    break;
                case WistInstruction.WistOperation.CallVariable:
                    Ldref(il);
                    il.Call(_getExeHelper);
                    il.Dup();
                    il.Call(_getMethodPtr);

                    var parameterTypes = GetParameters((int)(consts1[i1].GetNumber() + 0.1));
                    il.Calli(CallingConventions.Standard, typeof(WistConst), parameterTypes);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(inst.Op.ToString());
            }
        }
    }

    private static Type[] GetParameters(int count) =>
        Enumerable.Repeat(typeof(WistConst), count).Append(typeof(WistExecutionHelper)).ToArray();

    private static void Ldref(GroboIL il)
    {
        var loc = il.DeclareLocal(typeof(WistConst));
        il.Stloc(loc);
        il.Ldloca(loc);
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