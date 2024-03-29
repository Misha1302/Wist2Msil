namespace Wist2Msil;

using System.Diagnostics;
using WistConst;

[DebuggerDisplay("{Op}:{Constant}")]
public struct WistInstruction
{
    public readonly WistOperation Op;
    public readonly int Line;
    public WistConst Constant;
    public WistConst Constant2;

    public WistInstruction(WistOperation op, int line, WistConst constant = default, WistConst constant2 = default)
    {
        Op = op;
        Line = line;
        Constant = constant;
        Constant2 = constant2;
    }


    public enum WistOperation
    {
        PushConst,
        Add,
        CSharpCall,
        Sub,
        Mul,
        Div,
        Rem,
        LessThan,
        GreaterThan,
        LessThanOrEquals,
        GreaterThanOrEquals,
        Pow,
        SetLabel,
        Goto,
        Drop,
        Dup,
        Cmp,
        GotoIfFalse,
        GotoIfTrue,
        NegCmp,
        LoadLocal,
        SetLocal,
        Call,
        Ret,
        Instantiate,
        SetField,
        PushField,
        CallStructMethod,
        InstantiateList,
        Current,
        GotoIfNext,
        InstantiateFunctionPtr,
        CallVariable
    }
}