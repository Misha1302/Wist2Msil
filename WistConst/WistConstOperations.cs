namespace WistConst;

using System.Globalization;
using System.Runtime.CompilerServices;
using WistError;

public static class WistConstOperations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThan(in WistConst b, in WistConst a) => new(a.GetNumber() < b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThan(in WistConst b, in WistConst a) => new(a.GetNumber() > b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThanOrEquals(in WistConst b, in WistConst a) => new(a.GetNumber() <= b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThanOrEquals(in WistConst b, in WistConst a) => new(a.GetNumber() >= b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Equals(in WistConst b, in WistConst a)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(Math.Abs(a.GetNumber() - b.GetNumber()) < 0.001),
            WistType.String => new WistConst(a.GetString() == b.GetString()),
            WistType.Bool => new WistConst(a.GetBool() == b.GetBool()),
            WistType.List => new WistConst(a.GetList().SequenceEqual(b.GetList())),
            _ => Throw($"Cannot compare types: {a.Type} and {b.Type}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst NotEquals(in WistConst b, in WistConst a)
    {
        return a.Type switch
        {
            WistType.Number => new WistConst(Math.Abs(a.GetNumber() - b.GetNumber()) >= 0.001),
            WistType.String => new WistConst(a.GetString() != b.GetString()),
            WistType.Bool => new WistConst(a.GetBool() != b.GetBool()),
            WistType.List => new WistConst(!a.GetList().SequenceEqual(b.GetList())),
            _ => Throw($"Cannot compare types: {a.Type} and {b.Type}")
        };
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Rem(in WistConst b, in WistConst a) => new(a.GetNumber() % b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Add(in WistConst b, in WistConst a) => new(a.GetNumber() + b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Sub(in WistConst b, in WistConst a) => new(a.GetNumber() - b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Mul(in WistConst b, in WistConst a) => new(a.GetNumber() * b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Pow(in WistConst b, in WistConst a) => new(Math.Pow(a.GetNumber(), b.GetNumber()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Div(in WistConst b, in WistConst a) => new(a.GetNumber() / b.GetNumber());

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WistConst Throw(string s) => throw new WistError(s);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToStr(WistConst c)
    {
        return c.Type switch
        {
            WistType.Bool => c.GetBool().ToString(),
            WistType.Number => NumberToString(c.GetNumber()),
            WistType.String => c.GetString(),
            WistType.List => string.Join(", ", c.GetList()),
            WistType.None => "<<None>>",
            WistType.InternalInteger => $"i32_{c.GetInternalInteger()}",
            WistType.Pointer => $"ptr_{c.GetPointer()}",
            _ => Throw($"Unknown type to convert to string - {c.Type}").ToString()
        };
    }

    private static string NumberToString(double number) =>
        number.ToString(Math.Abs(number) < 1e10 ? "0.########" : "e", CultureInfo.InvariantCulture);
}