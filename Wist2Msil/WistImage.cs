namespace Wist2Msil;

using System.Reflection;
using WistConst;
using wInst = WistInstruction;

public sealed class WistImage
{
    private readonly List<WistInstruction> _instructions = new();
    public readonly List<string> Locals = new();
    private readonly HashSet<string> _localsHashSet = new();
    private WistFunction _function = null!;
    public IReadOnlyList<WistInstruction> Instructions => _instructions;

    public void PushConst(WistConst c) =>
        _instructions.Add(new wInst(WistInstruction.WistOperation.PushConst, c));

    public void Add() => _instructions.Add(new wInst(WistInstruction.WistOperation.Add));
    public void Sub() => _instructions.Add(new wInst(WistInstruction.WistOperation.Sub));
    public void Mul() => _instructions.Add(new wInst(WistInstruction.WistOperation.Mul));
    public void Div() => _instructions.Add(new wInst(WistInstruction.WistOperation.Div));
    public void Rem() => _instructions.Add(new wInst(WistInstruction.WistOperation.Rem));
    public void Pow() => _instructions.Add(new wInst(WistInstruction.WistOperation.Pow));

    public void GreaterThan() => _instructions.Add(new wInst(WistInstruction.WistOperation.GreaterThan));

    public void GreaterThanOrEquals() =>
        _instructions.Add(new wInst(WistInstruction.WistOperation.GreaterThanOrEquals));

    public void LessThan() => _instructions.Add(new wInst(WistInstruction.WistOperation.LessThan));
    public void LessThanOrEquals() => _instructions.Add(new wInst(WistInstruction.WistOperation.LessThanOrEquals));


    public void Call(MethodInfo? m)
    {
        if (m is null)
            throw new InvalidOperationException();

        _instructions.Add(
            new wInst(
                WistInstruction.WistOperation.CSharpCall,
                new WistConst(m)
            )
        );
    }

    public void SetLabel(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.WistOperation.SetLabel, new WistConst(labelName)));
    }

    public void Goto(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.WistOperation.Goto, new WistConst(labelName)));
    }

    public void Drop()
    {
        _instructions.Add(new wInst(WistInstruction.WistOperation.Drop));
    }

    public void Dup()
    {
        _instructions.Add(new wInst(WistInstruction.WistOperation.Dup));
    }

    public void Cmp()
    {
        _instructions.Add(new wInst(WistInstruction.WistOperation.Cmp));
    }

    public void NegCmp()
    {
        _instructions.Add(new wInst(WistInstruction.WistOperation.NegCmp));
    }

    public void GotoIfFalse(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.WistOperation.GotoIfFalse, new WistConst(labelName)));
    }

    public void SetLocal(string locName)
    {
        if (!_function.Parameters.Contains(locName))
            TryAddLoc(locName);

        _instructions.Add(new wInst(WistInstruction.WistOperation.SetLocal, new WistConst(locName)));
    }

    private void TryAddLoc(string locName)
    {
        if (_localsHashSet.Contains(locName)) return;

        _localsHashSet.Add(locName);
        Locals.Add(locName);
    }

    public void LoadLocal(string locName)
    {
        _instructions.Add(new wInst(WistInstruction.WistOperation.LoadLocal, new WistConst(locName)));
    }

    public void Call(WistFunction square)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.WistOperation.Call,
                default,
                new WistConst(square.Name.FullName)
            )
        );
    }

    public void Ret()
    {
        _instructions.Add(new wInst(WistInstruction.WistOperation.Ret));
    }

    public void LoadArg(string argName)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.WistOperation.LoadArg,
                new WistConst(argName)
            )
        );
    }

    public void Instantiate(WistCompilationStruct mCompilationStruct)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.WistOperation.Instantiate,
                new WistConst(mCompilationStruct)
            )
        );
    }

    public void SetField(string fieldName)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.WistOperation.SetField,
                new WistConst(fieldName)
            )
        );
    }

    public void PushField(string fieldName)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.WistOperation.PushField,
                new WistConst(fieldName)
            )
        );
    }

    public void Call(string mName, int argsLen)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.WistOperation.CallStructMethod,
                new WistConst(mName),
                WistConst.CreateInternalConst(argsLen)
            )
        );
    }

    public WistImage SetFunction(WistFunction function)
    {
        _function = function;
        return this;
    }

    public void InstantiateList()
    {
        _instructions.Add(
            new wInst(
                WistInstruction.WistOperation.InstantiateList
            )
        );
    }
}