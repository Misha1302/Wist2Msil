namespace WistList;

using Wist2MsilFrontend;
using WistConst;

[WistLibrary]
public static class WistList
{
    [WistLibraryFunction]
    public static WistConst ListAdd(WistConst list, WistConst elem)
    {
        list.Get<WistFastList.WistFastList<WistConst>>().Add(elem);
        return WistConst.CreateNull();
    }

    [WistLibraryFunction]
    public static WistConst ListRemove(WistConst list, WistConst elem)
    {
        list.Get<WistFastList.WistFastList<WistConst>>().Remove(elem);
        return WistConst.CreateNull();
    }

    [WistLibraryFunction]
    public static WistConst ListRemoveAt(WistConst list, WistConst ind)
    {
        list.Get<WistFastList.WistFastList<WistConst>>().RemoveAt((int)(ind.GetNumber() + 0.1));
        return WistConst.CreateNull();
    }

    [WistLibraryFunction]
    public static WistConst GetElemFromList(WistConst list, WistConst ind) =>
        list.Get<WistFastList.WistFastList<WistConst>>()[(int)(ind.GetNumber() + 0.1)];

    [WistLibraryFunction]
    public static WistConst GetListLen(WistConst list) =>
        new(list.Get<WistFastList.WistFastList<WistConst>>().Count);

    [WistLibraryFunction]
    public static WistConst SetElemInList(WistConst list, WistConst elem, WistConst ind)
    {
        list.Get<WistFastList.WistFastList<WistConst>>()[(int)(ind.GetNumber() + 0.1)] = elem;
        return WistConst.CreateNull();
    }

    [WistLibraryFunction]
    public static WistConst ListInsert(WistConst list, WistConst ind, WistConst elem)
    {
        list.Get<WistFastList.WistFastList<WistConst>>().Insert((int)(ind.GetNumber() + 0.1), elem);
        return WistConst.CreateNull();
    }
}