namespace WistConst;

using System.Globalization;
using System.Runtime.CompilerServices;
using WistError;
using WistFastList;

public static class WistConstOperations
{
    private static readonly WistConst _true = new(true);
    private static readonly WistConst _false = new(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThan(in WistConst a, in WistConst b) =>
        a.GetNumber() < b.GetNumber() ? _true : _false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThan(in WistConst a, in WistConst b) =>
        a.GetNumber() > b.GetNumber() ? _true : _false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst LessThanOrEquals(in WistConst a, in WistConst b) =>
        a.GetNumber() <= b.GetNumber() ? _true : _false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst GreaterThanOrEquals(in WistConst a, in WistConst b) =>
        a.GetNumber() >= b.GetNumber() ? _true : _false;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Rem(in WistConst a, in WistConst b) => new(a.GetNumber() % b.GetNumber());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Add(in WistConst a, in WistConst b) =>
        a.Type == WistType.Number
            ? new WistConst(a.GetNumber() + b.GetNumber())
            : new WistConst(a.Get<string>() + b.Get<string>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Sub(in WistConst a, in WistConst b) =>
        a.Type == WistType.Number
            ? new WistConst(a.GetNumber() - b.GetNumber())
            : new WistConst(Replace(a.Get<string>(), b.Get<string>(), string.Empty));

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
            WistType.String => c.Get<string>(),
            WistType.List => string.Join(", ", c.Get<WistFastList<WistConst>>()),
            WistType.None => "<<None>>",
            WistType.InternalInteger => $"i32_{c.GetInternalInteger()}",
            WistType.Pointer => $"ptr_{c.GetPointer()}",
            WistType.Struct => "<<Struct>>",
            WistType.Null => "<<Null>>",
            _ => Throw($"Unknown type to convert to string - {c.Type}").ToString()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string NumberToString(double number) =>
        number.ToString(Math.Abs(number) < 1e10 ? "0.########" : "e", CultureInfo.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string Replace(string src, string tar, string value)
    {
        var srcInternal = src;

        var index = 0;
        while ((index = srcInternal.IndexOf(tar, index, StringComparison.Ordinal)) != -1)
            srcInternal = srcInternal.Remove(index, tar.Length).Insert(index, value);

        return srcInternal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst Cmp(WistConst c, WistConst c1) => c == c1 ? _true : _false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst NotCmp(WistConst c, WistConst c1) => c != c1 ? _true : _false;
}