namespace Wist2MsilFrontend;

using Antlr4.Runtime.Tree;
using Wist2Msil;
using Wist2MsilFrontend.Content;

public sealed class WistFunctionsVisitor : WistGrammarBaseVisitor<object?>
{
    private readonly List<WistFunction> _list = new();

    public List<WistFunction> GetAllFunctions(IParseTree parseTree) =>
        (List<WistFunction>)Visit(parseTree);

    public override object VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        var name = context.IDENTIFIER(0).GetText();
        var args = context.IDENTIFIER().Skip(1).Select(x => x.GetText()).ToArray();

        _list.Add(new WistFunction(name, new WistImage(), args));
        return _list;
    }

    public override object Visit(IParseTree tree)
    {
        base.Visit(tree);
        return _list;
    }
}