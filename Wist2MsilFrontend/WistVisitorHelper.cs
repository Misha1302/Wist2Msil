namespace Wist2MsilFrontend;

using WistConst;

public static class WistVisitorHelper
{
    public static WistConst AddToList(WistConst list, WistConst elem)
    {
        list.GetList().Add(elem);
        return WistConst.CreateNull();
    }
}