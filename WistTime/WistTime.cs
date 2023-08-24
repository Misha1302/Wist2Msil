namespace WistTime;

using Wist2MsilFrontend;
using WistConst;

[WistLibrary]
public static class WistTime
{
    [WistFunction]
    public static WistConst GetTimeInMs() => new(DateTimeOffset.Now.ToUnixTimeMilliseconds());
}