namespace Wist2MsilFrontend;

using WistConst;

public static class WistVisitorHelper
{
    public static WistConst AddToList(WistConst list, WistConst elem)
    {
        list.Get<List<WistConst>>().Add(elem);
        return WistConst.CreateNull();
    }
}