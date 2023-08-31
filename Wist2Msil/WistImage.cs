namespace Wist2Msil;

using System.Reflection;
using WistConst;
using WistFastList;
using wInst = WistInstruction;

public sealed class WistImage
{
    public readonly WistFastList<string> Locals = new();
    private readonly HashSet<string> _localsHashSet = new();
    private WistFunction _function = null!;
    public WistFastList<WistInstruction> Instructions { get; } = new();

    public void PushConst(WistConst c) =>
        Instructions.Add(new wInst(WistInstruction.WistOperation.PushConst, c));

    public void Add() => Instructions.Add(new wInst(WistInstruction.WistOperation.Add));
    public void Sub() => Instructions.Add(new wInst(WistInstruction.WistOperation.Sub));
    public void Mul() => Instructions.Add(new wInst(WistInstruction.WistOperation.Mul));
    public void Div() => Instructions.Add(new wInst(WistInstruction.WistOperation.Div));
    public void Rem() => Instructions.Add(new wInst(WistInstruction.WistOperation.Rem));
    public void Pow() => Instructions.Add(new wInst(WistInstruction.WistOperation.Pow));

    public void GreaterThan() => Instructions.Add(new wInst(WistInstruction.WistOperation.GreaterThan));

    public void GreaterThanOrEquals() =>
        Instructions.Add(new wInst(WistInstruction.WistOperation.GreaterThanOrEquals));

    public void LessThan() => Instructions.Add(new wInst(WistInstruction.WistOperation.LessThan));
    public void LessThanOrEquals() => Instructions.Add(new wInst(WistInstruction.WistOperation.LessThanOrEquals));


    public void Call(MethodInfo? m)
    {
        if (m is null)
            throw new InvalidOperationException();

        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.CSharpCall,
                new WistConst(m)
            )
        );
    }

    public void SetLabel(string labelName)
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.SetLabel, new WistConst(labelName)));
    }

    public void Goto(string labelName)
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.Goto, new WistConst(labelName)));
    }

    public void Drop()
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.Drop));
    }

    public void Dup()
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.Dup));
    }

    public void Cmp()
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.Cmp));
    }

    public void NegCmp()
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.NegCmp));
    }

    public void GotoIfFalse(string labelName)
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.GotoIfFalse, new WistConst(labelName)));
    }

    public void SetLocal(string locName)
    {
        if (!_function.Parameters.Contains(locName))
            TryAddLoc(locName);

        Instructions.Add(new wInst(WistInstruction.WistOperation.SetLocal, new WistConst(locName)));
    }

    private void TryAddLoc(string locName)
    {
        if (_localsHashSet.Contains(locName)) return;

        _localsHashSet.Add(locName);
        Locals.Add(locName);
    }

    public void LoadLocal(string locName)
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.LoadLocal, new WistConst(locName)));
    }

    public void Call(WistFunction square)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.Call,
                default,
                new WistConst(square.Name.FullName)
            )
        );
    }

    public void Ret()
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.Ret));
    }

    public void LoadArg(string argName)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.LoadArg,
                new WistConst(argName)
            )
        );
    }

    public void Instantiate(WistCompilationStruct mCompilationStruct)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.Instantiate,
                new WistConst(mCompilationStruct)
            )
        );
    }

    public void SetField(string fieldName)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.SetField,
                new WistConst(fieldName)
            )
        );
    }

    public void PushField(string fieldName)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.PushField,
                new WistConst(fieldName)
            )
        );
    }

    public void Call(string mName, int argsLen)
    {
        Instructions.Add(
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
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.InstantiateList
            )
        );
    }
}