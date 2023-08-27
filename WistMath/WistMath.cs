namespace WistMath;

using Wist2MsilFrontend;
using WistConst;

[WistLibrary]
public static class WistMath
{
    [WistLibraryFunction]
    public static WistConst Sin(WistConst c) => new(Math.Sin(c.GetNumber()));

    [WistLibraryFunction]
    public static WistConst Cos(WistConst c) => new(Math.Cos(c.GetNumber()));

    [WistLibraryFunction]
    public static WistConst Abs(WistConst c) => new(Math.Abs(c.GetNumber()));

    [WistLibraryFunction]
    public static WistConst Min(WistConst c, WistConst c1) => new(Math.Min(c.GetNumber(), c1.GetNumber()));

    [WistLibraryFunction]
    public static WistConst Max(WistConst c, WistConst c1) => new(Math.Max(c.GetNumber(), c1.GetNumber()));
}