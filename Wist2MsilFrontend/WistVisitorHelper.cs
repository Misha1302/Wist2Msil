namespace Wist2MsilFrontend;

using System.Runtime.CompilerServices;
using WistConst;

public static class WistVisitorHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WistConst AddToList(WistConst s, WistConst el)
    {
        s.GetWistFastList().Add(el);

        return WistConst.CreateNull();
    }

    public static WistConst InstantiateRepeatEnumerator(WistConst start, WistConst max, WistConst step) =>
        new(new WistRepeatEnumerator(start.To32(), max.To32(), step.To32()));

    private static int To32(this WistConst c) => (int)(c.GetNumber() + 0.1);
}