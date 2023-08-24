namespace WistConst;

using System.Globalization;
using System.Runtime.CompilerServices;
using WistError;

public static class WistConstOperations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThan(in WistConst a, in WistConst b) => new(a.GetNumber() < b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThan(in WistConst a, in WistConst b) => new(a.GetNumber() > b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThanOrEquals(in WistConst a, in WistConst b) => new(a.GetNumber() <= b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThanOrEquals(in WistConst a, in WistConst b) => new(a.GetNumber() >= b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Equals(in WistConst a, in WistConst b)
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
    public static WistConst NotEquals(in WistConst a, in WistConst b)
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
    public static WistConst Rem(in WistConst a, in WistConst b) => new(a.GetNumber() % b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Add(in WistConst a, in WistConst b) =>
        a.Type == WistType.Number
            ? new WistConst(a.GetNumber() + b.GetNumber())
            : new WistConst(a.GetString() + b.GetString());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Sub(in WistConst a, in WistConst b) =>
        a.Type == WistType.Number
            ? new WistConst(a.GetNumber() - b.GetNumber())
            : new WistConst(Replace(a.GetString(), b.GetString(), string.Empty));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Mul(in WistConst a, in WistConst b) => new(a.GetNumber() * b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Pow(in WistConst a, in WistConst b) => new(Math.Pow(a.GetNumber(), b.GetNumber()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Div(in WistConst a, in WistConst b) => new(a.GetNumber() / b.GetNumber());

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
            WistType.Struct => $"<<Struct>>",
            _ => Throw($"Unknown type to convert to string - {c.Type}").ToString()
        };
    }

    private static string NumberToString(double number) =>
        number.ToString(Math.Abs(number) < 1e10 ? "0.########" : "e", CultureInfo.InvariantCulture);

    private static string Replace(string src, string tar, string value)
    {
        var srcInternal = src;

        var index = 0;
        while ((index = srcInternal.IndexOf(tar, index, StringComparison.Ordinal)) != -1)
            srcInternal = srcInternal.Remove(index, tar.Length).Insert(index, value);

        return srcInternal;
    }
}