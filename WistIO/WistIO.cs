namespace WistIO;

using Wist2Msil;
using Wist2MsilFrontend;
using WistConst;

[WistLibrary]
// ReSharper disable once InconsistentNaming
public static class WistIO
{
    [WistLibraryFunction]
    public static WistConst Print(WistConst c)
    {
        Console.WriteLine(c.ToString());
        return WistConst.CreateNull();
    }

    [WistLibraryFunction]
    public static WistConst ToStr(WistConst c) => new(c.ToString());

    [WistLibraryFunction]
    public static WistConst InputString() => new(Console.ReadLine() ?? "\n");

    [WistLibraryFunction]
    public static WistConst InputNumber() => new((Console.ReadLine() ?? string.Empty).ToDouble());

    [WistLibraryFunction]
    public static WistConst InputBool() => new((Console.ReadLine() ?? string.Empty).ToBool());
}