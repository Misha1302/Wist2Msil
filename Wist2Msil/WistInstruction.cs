namespace Wist2Msil;

public struct WistInstruction
{
    public readonly Operation Op;
    public WistConst.WistConst Constant;
    public WistConst.WistConst Constant2;

    public WistInstruction(Operation op, WistConst.WistConst constant = default, WistConst.WistConst constant2 = default)
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
        PushField
    }
}