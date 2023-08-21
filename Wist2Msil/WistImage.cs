namespace Wist2Msil;

using System.Reflection;
using System.Runtime.CompilerServices;
using WistConst;
using wInst = WistInstruction;

public sealed class WistImage
{
    private readonly List<WistInstruction> _instructions = new();
    public readonly List<string> Locals = new();
    public readonly HashSet<string> LocalsHashSet = new();
    public IReadOnlyList<WistInstruction> Instructions => _instructions;

    public void PushConst(WistConst c) =>
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


    public void Call(MethodInfo? m)
    {
        if (m is null)
            throw new InvalidOperationException();

        RuntimeHelpers.PrepareMethod(m.MethodHandle);
        _instructions.Add(
            new wInst(
                WistInstruction.Operation.CSharpCall,
                WistConst.CreateInternalConst(m.MethodHandle.GetFunctionPointer()),
                WistConst.CreateInternalConst(m.GetParameters().Length)
            )
        );
    }

    public void SetLabel(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.Operation.SetLabel, new WistConst(labelName)));
    }

    public void Goto(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.Operation.Goto, new WistConst(labelName)));
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
        _instructions.Add(new wInst(WistInstruction.Operation.GotoIfFalse, new WistConst(labelName)));
    }

    public void GotoIfTrue(string labelName)
    {
        _instructions.Add(new wInst(WistInstruction.Operation.GotoIfTrue, new WistConst(labelName)));
    }

    public void SetLocal(string locName)
    {
        TryAddLoc(locName);
        _instructions.Add(new wInst(WistInstruction.Operation.SetLocal, new WistConst(locName)));
    }

    private void TryAddLoc(string locName)
    {
        if (LocalsHashSet.Contains(locName)) return;

        LocalsHashSet.Add(locName);
        Locals.Add(locName);
    }

    public void LoadLocal(string locName)
    {
        _instructions.Add(new wInst(WistInstruction.Operation.LoadLocal, new WistConst(locName)));
    }

    public void Call(WistFunction square)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.Operation.WistCall,
                default,
                new WistConst(square.Name)
            )
        );
    }

    public void Ret()
    {
        _instructions.Add(new wInst(WistInstruction.Operation.Ret));
    }

    public void LoadArg(string argName)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.Operation.LoadArg,
                new WistConst(argName)
            )
        );
    }

    public void Instantiate(WistCompilationStruct mCompilationStruct)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.Operation.Instantiate,
                new WistConst(mCompilationStruct)
            )
        );
    }

    public void SetField(string fieldName)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.Operation.SetField,
                new WistConst(fieldName)
            )
        );
    }

    public void PushField(string fieldName)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.Operation.PushField,
                new WistConst(fieldName)
            )
        );
    }

    public void Call(string mName, int argsLen)
    {
        _instructions.Add(
            new wInst(
                WistInstruction.Operation.CallStructMethod,
                new WistConst(mName),
                WistConst.CreateInternalConst(argsLen)
            )
        );
    }
}