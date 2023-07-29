namespace Wist2Msil;

using System.Reflection;
using System.Runtime.CompilerServices;
using wInst = WistInstruction;

public sealed class WistImage
{
    private readonly List<WistInstruction> _instructions = new();
    public IReadOnlyList<WistInstruction> Instructions => _instructions;

    public void PushConst(WistConst.WistConst c) =>
        _instructions.Add(new wInst(WistInstruction.Operation.PushConst, c));

    public void Add() => _instructions.Add(new wInst(WistInstruction.Operation.Add));
    public void Sub() => _instructions.Add(new wInst(WistInstruction.Operation.Sub));
    public void Mul() => _instructions.Add(new wInst(WistInstruction.Operation.Mul));
    public void Div() => _instructions.Add(new wInst(WistInstruction.Operation.Div));
    public void Rem() => _instructions.Add(new wInst(WistInstruction.Operation.Rem));
    public void Pow() => _instructions.Add(new wInst(WistInstruction.Operation.Pow));

    public void IsEquals() => _instructions.Add(new wInst(WistInstruction.Operation.IsEquals));
    public void IsNotEquals() => _instructions.Add(new wInst(WistInstruction.Operation.IsNotEquals));
    public void GreaterThan() => _instructions.Add(new wInst(WistInstruction.Operation.GreaterThan));
    public void GreaterThanOrEquals() => _instructions.Add(new wInst(WistInstruction.Operation.GreaterThanOrEquals));
    public void LessThan() => _instructions.Add(new wInst(WistInstruction.Operation.LessThan));
    public void LessThanOrEquals() => _instructions.Add(new wInst(WistInstruction.Operation.LessThanOrEquals));


    public void Call(MethodInfo m)
    {
        RuntimeHelpers.PrepareMethod(m.MethodHandle);
        _instructions.Add(new wInst(
            WistInstruction.Operation.Call,
            WistConst.WistConst.CreateInternalConst(m.MethodHandle.GetFunctionPointer()),
            WistConst.WistConst.CreateInternalConst(m.GetParameters().Length
            )));
    }

    public void SetLabel(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.Operation.SetLabel, new WistConst.WistConst(labelName)));
    }

    public void Goto(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.Operation.Goto, new WistConst.WistConst(labelName)));
    }

    public void Drop()
    {
        _instructions.Add(new wInst(WistInstruction.Operation.Drop));
    }

    public void Unite(WistImage anotherImage)
    {
        _instructions.AddRange(anotherImage._instructions);
    }

    public void Dup()
    {
        _instructions.Add(new wInst(WistInstruction.Operation.Dup));
    }

    public void Cmp()
    {
        _instructions.Add(new wInst(WistInstruction.Operation.Cmp));
    }

    public void NegCmp()
    {
        _instructions.Add(new wInst(WistInstruction.Operation.NegCmp));
    }

    public void GotoIfFalse(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.Operation.GotoIfFalse, new WistConst.WistConst(labelName)));
    }

    public void GotoIfTrue(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.Operation.GotoIfTrue, new WistConst.WistConst(labelName)));
    }
}