namespace Wist2Msil.WistHashCode;

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

public sealed class WistHashCode
{
    private static WistHashCode? _instance;
    private readonly Dictionary<string, int> _hashes = new();

    public static WistHashCode Instance => _instance ??= new WistHashCode();

    public string GetSourceString(int hash)
    {
        return _hashes.First(x => x.Value == hash).Key;
    }

    public int GetHashCode(string s)
    {
        var sum = BitConverter.ToInt32(MD5.HashData(Encoding.UTF8.GetBytes(s)));
        _hashes.TryAdd(s, sum);
        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(long l) => unchecked((int)l ^ (int)(l >> 32));
}