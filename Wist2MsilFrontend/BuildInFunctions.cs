namespace Wist2MsilFrontend;

using Wist2Msil;
using WistConst;

[WistLibrary]
public static class WistBuildInFunctions
{
    [WistFunction]
    public static WistConst Print(WistConst c)
    {
        Console.WriteLine(c.ToString());
        return default;
    }

    [WistFunction]
    public static WistConst AddToList(WistConst list, WistConst elem)
    {
        list.GetList().Add(elem);
        return default;
    }

    [WistFunction]
    public static WistConst ToStr(WistConst c) => new(c.ToString());

    [WistFunction]
    public static WistConst InputString() => new(Console.ReadLine() ?? "\n");

    [WistFunction]
    public static WistConst InputNumber() => new((Console.ReadLine() ?? string.Empty).ToDouble());

    [WistFunction]
    public static WistConst InputBool() => new((Console.ReadLine() ?? string.Empty).ToBool());
}