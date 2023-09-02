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
    private Func<int> _getCurLine = null!;
    public WistFastList<WistInstruction> Instructions { get; } = new();

    public void PushConst(WistConst c) =>
        Instructions.Add(new wInst(WistInstruction.WistOperation.PushConst, _getCurLine(), c));

    public void Add() => Instructions.Add(new wInst(WistInstruction.WistOperation.Add, _getCurLine()));
    public void Sub() => Instructions.Add(new wInst(WistInstruction.WistOperation.Sub, _getCurLine()));
    public void Mul() => Instructions.Add(new wInst(WistInstruction.WistOperation.Mul, _getCurLine()));
    public void Div() => Instructions.Add(new wInst(WistInstruction.WistOperation.Div, _getCurLine()));
    public void Rem() => Instructions.Add(new wInst(WistInstruction.WistOperation.Rem, _getCurLine()));
    public void Pow() => Instructions.Add(new wInst(WistInstruction.WistOperation.Pow, _getCurLine()));

    public void GreaterThan() => Instructions.Add(new wInst(WistInstruction.WistOperation.GreaterThan, _getCurLine()));

    public void GreaterThanOrEquals() =>
        Instructions.Add(new wInst(WistInstruction.WistOperation.GreaterThanOrEquals, _getCurLine()));

    public void LessThan() => Instructions.Add(new wInst(WistInstruction.WistOperation.LessThan, _getCurLine()));

    public void LessThanOrEquals() =>
        Instructions.Add(new wInst(WistInstruction.WistOperation.LessThanOrEquals, _getCurLine()));


    public void Call(MethodInfo? m, WistConst secondParam = default)
    {
        if (m is null)
            throw new InvalidOperationException();

        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.CSharpCall,
                _getCurLine(),
                new WistConst(m),
                secondParam
            )
        );
    }

    public void SetLabel(string labelName)
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.SetLabel, _getCurLine(), new WistConst(labelName)));
    }

    public void Goto(string labelName)
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.Goto, _getCurLine(), new WistConst(labelName)));
    }

    public void Drop()
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.Drop, _getCurLine()));
    }

    public void Dup()
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.Dup, _getCurLine()));
    }

    public void Cmp()
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.Cmp, _getCurLine()));
    }

    public void NegCmp()
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.NegCmp, _getCurLine()));
    }

    public void GotoIfFalse(string labelName)
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.GotoIfFalse, _getCurLine(), new WistConst(labelName)));
    }

    public void SetLocal(string locName)
    {
        if (!_function.Parameters.Contains(locName))
            TryAddLoc(locName);

        Instructions.Add(new wInst(WistInstruction.WistOperation.SetLocal, _getCurLine(), new WistConst(locName)));
    }

    private void TryAddLoc(string locName)
    {
        if (_localsHashSet.Contains(locName)) return;

        _localsHashSet.Add(locName);
        Locals.Add(locName);
    }

    public void LoadLocal(string locName)
    {
        Instructions.Add(new wInst(WistInstruction.WistOperation.LoadLocal, _getCurLine(), new WistConst(locName)));
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
        Instructions.Add(new wInst(WistInstruction.WistOperation.Ret, _getCurLine()));
    }

    public void Instantiate(WistCompilationStruct mCompilationStruct)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.Instantiate,
                _getCurLine(),
                new WistConst(mCompilationStruct)
            )
        );
    }

    public void SetField(string fieldName)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.SetField,
                _getCurLine(),
                new WistConst(fieldName)
            )
        );
    }

    public void PushField(string fieldName)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.PushField,
                _getCurLine(),
                new WistConst(fieldName)
            )
        );
    }

    public void Call(string mName, int argsLen)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.CallStructMethod,
                _getCurLine(),
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

    public void InstantiateList(int length)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.InstantiateList,
                _getCurLine(),
                new WistConst(length)
            )
        );
    }

    public void Current()
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.Current,
                _getCurLine()
            )
        );
    }

    public void GotoIfNext(string labelName)
    {
        Instructions.Add(
            new wInst(
                WistInstruction.WistOperation.GotoIfNext,
                _getCurLine(),
                new WistConst(labelName)
            )
        );
    }

    public void InitGetLineAction(Func<int> getCurLine) => _getCurLine = getCurLine;
}