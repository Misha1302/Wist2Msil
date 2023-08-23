namespace WistMath;

using Wist2MsilFrontend;
using WistConst;

[WistLibrary]
public static class WistMath
{
    [WistFunction]
    public static WistConst Sin(WistConst c) => new(Math.Sin(c.GetNumber()));

    [WistFunction]
    public static WistConst Cos(WistConst c) => new(Math.Cos(c.GetNumber()));

    [WistFunction]
    public static WistConst Abs(WistConst c) => new(Math.Abs(c.GetNumber()));

    [WistFunction]
    public static WistConst Min(WistConst c, WistConst c1) => new(Math.Min(c.GetNumber(), c1.GetNumber()));

    [WistFunction]
    public static WistConst Max(WistConst c, WistConst c1) => new(Math.Max(c.GetNumber(), c1.GetNumber()));
}