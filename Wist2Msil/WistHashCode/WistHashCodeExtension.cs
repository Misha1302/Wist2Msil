namespace Wist2Msil.WistHashCode;

using System.Runtime.CompilerServices;

public static class WistHashCodeExtension
{
    public static int GetWistHashCode(this string s) => WistHashCode.Instance.GetHashCode(s);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetWistHashCode(this long s) => WistHashCode.GetHashCode(s);
}