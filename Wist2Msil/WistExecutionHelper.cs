namespace Wist2Msil;

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using WistConst;

public sealed unsafe class WistExecutionHelper
{
    private const WistConst.Capacity Capacity = WistConst.Capacity.C31;
    private readonly ImmutableArray<WistConst> _consts;
    private WistStack<WistConst> _stack = new(Capacity);

    public WistExecutionHelper(IEnumerable<WistConst> consts)
    {
        _consts = consts.ToImmutableArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PushConst(int index) => _stack.Push(_consts[index]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add() => _stack.Push(WistConstOperations.Add(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sub() => _stack.Push(WistConstOperations.Sub(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Mul() => _stack.Push(WistConstOperations.Mul(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Div() => _stack.Push(WistConstOperations.Div(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Rem() => _stack.Push(WistConstOperations.Rem(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Pow() => _stack.Push(WistConstOperations.Pow(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LessThan() => _stack.Push(WistConstOperations.LessThan(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LessThanOrEquals() => _stack.Push(WistConstOperations.LessThanOrEquals(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GreaterThan() => _stack.Push(WistConstOperations.GreaterThan(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GreaterThanOrEquals() =>
        _stack.Push(WistConstOperations.GreaterThanOrEquals(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IsEquals() => _stack.Push(WistConstOperations.Equals(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IsNotEquals() => _stack.Push(WistConstOperations.NotEquals(_stack.Pop(), _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Drop() => _stack.Drop();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dup() => _stack.Dup();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once EqualExpressionComparison
    public void Cmp() => _stack.Push(new WistConst(_stack.Pop() == _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // ReSharper disable once EqualExpressionComparison
    public void NegCmp() => _stack.Push(new WistConst(_stack.Pop() != _stack.Pop()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PopBool() => _stack.Pop().GetBool();

    public WistConst PushDefaultConst() => default;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Call0(int index)
    {
        var pointer = (delegate*<WistConst>)_consts[index].GetPointer();
        _stack.Push(pointer());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Call1(int index)
    {
        var pointer = (delegate*<WistConst, WistConst>)_consts[index].GetPointer();
        _stack.Push(pointer(_stack.Pop()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Call2(int index)
    {
        var pointer =
            (delegate*<WistConst, WistConst, WistConst>)_consts[index].GetPointer();
        _stack.Push(pointer(_stack.Pop(), _stack.Pop()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Call3(int index)
    {
        var pointer =
            (delegate*<WistConst, WistConst, WistConst, WistConst>)
            _consts[index].GetPointer();
        _stack.Push(pointer(_stack.Pop(), _stack.Pop(), _stack.Pop()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Call4(int index)
    {
        var pointer =
            (delegate*<WistConst, WistConst, WistConst, WistConst,
                WistConst>)_consts[index].GetPointer();
        _stack.Push(pointer(_stack.Pop(), _stack.Pop(), _stack.Pop(), _stack.Pop()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Call5(int index)
    {
        var pointer =
            (delegate*<WistConst, WistConst, WistConst, WistConst,
                WistConst, WistConst>)_consts[index].GetPointer();
        _stack.Push(pointer(_stack.Pop(), _stack.Pop(), _stack.Pop(), _stack.Pop(), _stack.Pop()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Call6(int index)
    {
        var pointer =
            (delegate*<WistConst, WistConst, WistConst, WistConst,
                WistConst, WistConst, WistConst>)_consts[index]
                .GetPointer();
        _stack.Push(pointer(_stack.Pop(), _stack.Pop(), _stack.Pop(), _stack.Pop(), _stack.Pop(), _stack.Pop()));
    }

    public void Reset()
    {
        _stack = new WistStack<WistConst>(Capacity);
    }
}