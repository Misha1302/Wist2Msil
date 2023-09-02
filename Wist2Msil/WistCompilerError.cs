namespace Wist2Msil;

using WistFuncName;

public sealed class WistCompilerError : Exception
{
    public readonly int Line;
    public readonly WistFuncName FuncFullName;

    public WistCompilerError(int line, WistFuncName funcNameFullName)
    {
        Line = line;
        FuncFullName = funcNameFullName;
    }
}