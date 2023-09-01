namespace Wist2MsilFrontend;

using Antlr4.Runtime;
using WistError;

public sealed class WistErrorListener : IAntlrErrorListener<int>, IAntlrErrorListener<IToken>
{
    private readonly List<WistError> _errors;
    private readonly string _prefix;

    public WistErrorListener(string prefix, List<WistError> errors)
    {
        _prefix = prefix;
        _errors = errors;
    }

    public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg,
        RecognitionException e)
    {
        _errors.Add(new WistError(_prefix + $"Error in line {line}. Msg: {FormatMsg(msg)}"));
    }

    public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine,
        string msg,
        RecognitionException e)
    {
        _errors.Add(new WistError(_prefix + $"Error in line {line}. Msg: {FormatMsg(msg)}"));
    }


    private static string FormatMsg(string msg) => msg.Replace("'\n'", "'\\n'").Replace("'\r'", "'\\r'");
}