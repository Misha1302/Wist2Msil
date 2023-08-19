namespace Wist2Msil;

public sealed class WistCompilationStruct
{
    public readonly string Name;
    public readonly string[] Fields;
    public readonly string[] Methods;

    public WistCompilationStruct(string name, string[] fields, string[] methods)
    {
        Name = name;
        Fields = fields;
        Methods = methods;
    }
}