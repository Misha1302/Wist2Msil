namespace WistTime;

using Wist2MsilFrontend;
using WistConst;

[WistLibrary]
public static class WistTime
{
    [WistLibraryFunction]
    public static WistConst GetTimeInMs() => new(DateTimeOffset.Now.ToUnixTimeMilliseconds());
}