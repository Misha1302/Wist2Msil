namespace Wist2Msil;

using WistConst;

public struct WistInstruction
{
    public readonly Operation Op;
    public WistConst Constant;
    public WistConst Constant2;

    public WistInstruction(Operation op, WistConst constant = default, WistConst constant2 = default)
    {
        Op = op;
        Constant = constant;
        Constant2 = constant2;
    }


    public enum Operation
    {
        PushConst,
        Add,
        CSharpCall,
        Sub,
        Mul,
        Div,
        Rem,
        IsEquals,
        IsNotEquals,
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
        WistCall,
        LoadArg,
        Ret,
        Instantiate,
        SetField,
        PushField,
        CallStructMethod
    }
}