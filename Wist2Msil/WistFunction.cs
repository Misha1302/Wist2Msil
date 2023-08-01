namespace Wist2Msil;

public sealed class WistFunction
{
    public readonly WistImage Image;
    public readonly string Name;
    public readonly string[] Parameters;

    public WistFunction(string name, WistImage image, string[] parameters)
    {
        Name = name;
        Image = image;
        Parameters = parameters;
    }
}