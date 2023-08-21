namespace Wist2Msil;

using WistConst;

public static class BuildInFunctions
{
    public static WistConst Print(WistConst c)
    {
        Console.WriteLine(c.ToString());
        return default;
    }

    public static WistConst ToStr(WistConst c) => new(c.ToString());
    public static WistConst InputString() => new(Console.ReadLine() ?? "\n");
    public static WistConst InputNumber() => new((Console.ReadLine() ?? string.Empty).ToDouble());
    public static WistConst InputBool() => new((Console.ReadLine() ?? string.Empty).ToBool());
}