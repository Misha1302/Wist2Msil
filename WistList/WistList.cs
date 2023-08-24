﻿namespace WistList;

using Wist2MsilFrontend;
using WistConst;

[WistLibrary]
public static class WistList
{
    [WistFunction]
    public static WistConst ListAdd(WistConst list, WistConst elem)
    {
        list.GetList().Add(elem);
        return WistConst.CreateNull();
    }

    [WistFunction]
    public static WistConst ListRemove(WistConst list, WistConst elem)
    {
        list.GetList().Remove(elem);
        return WistConst.CreateNull();
    }

    [WistFunction]
    public static WistConst ListRemoveAt(WistConst list, WistConst ind)
    {
        list.GetList().RemoveAt((int)(ind.GetNumber() + 0.1));
        return WistConst.CreateNull();
    }

    [WistFunction]
    public static WistConst ListInsert(WistConst list, WistConst ind, WistConst elem)
    {
        list.GetList().Insert((int)(ind.GetNumber() + 0.1), elem);
        return WistConst.CreateNull();
    }
}