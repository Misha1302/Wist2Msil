namespace Wist2Msil;

using System.Diagnostics;
using WistFuncName;

[DebuggerDisplay("{Name}")]
public sealed class WistFunction
{
    public readonly WistImage Image;
    public readonly WistFuncName Name;
    public readonly string[] Parameters;

    public WistFunction(WistFuncName name, WistImage image, string[] parameters)
    {
        Name = name;
        Image = image;
        Parameters = parameters;
    }
}