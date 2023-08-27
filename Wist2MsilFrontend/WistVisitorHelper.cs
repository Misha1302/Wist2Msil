namespace Wist2MsilFrontend;

using System.Runtime.CompilerServices;
using WistConst;

public static class WistVisitorHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst AddToList(WistConst list, WistConst elem)
    {
        list.Get<List<WistConst>>().Add(elem);
        return WistConst.CreateNull();
    }
}