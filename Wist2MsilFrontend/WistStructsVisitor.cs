namespace Wist2MsilFrontend;

using Antlr4.Runtime.Tree;
using Wist2MsilFrontend.Content;
using WistConst;

public sealed class WistStructsVisitor : WistGrammarBaseVisitor<object?>
{
    private readonly List<WistCompilationStruct> _list = new();
    private List<string>? _methods;

    public List<WistCompilationStruct> GetAllStructs(IParseTree parseTree) =>
        (List<WistCompilationStruct>)Visit(parseTree);

    public override object? VisitStructDecl(WistGrammarParser.StructDeclContext context)
    {
        var name = context.IDENTIFIER(0).GetText();
        var fields = context.IDENTIFIER().Skip(1).Select(x => x.GetText()).ToArray();

        _methods = new List<string>();
        Visit(context.block());
        _list.Add(new WistCompilationStruct(name, fields, _methods.ToArray()));
        _methods = null;
        return null;
    }

    public override object? VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        _methods?.Add(context.IDENTIFIER(0).GetText());
        return null;
    }

    public override object Visit(IParseTree tree)
    {
        base.Visit(tree);
        return _list;
    }
}