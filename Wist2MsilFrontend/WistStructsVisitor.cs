namespace Wist2MsilFrontend;

using Antlr4.Runtime.Tree;
using Wist2MsilFrontend.Content;
using WistConst;
using WistFuncName;

public sealed class WistStructsVisitor : WistGrammarBaseVisitor<object?>
{
    private readonly List<WistCompilationStruct> _list = new();
    private List<WistFuncName>? _methods;
    private string? _curStructName;

    public List<WistCompilationStruct> GetAllStructs(IParseTree parseTree) =>
        (List<WistCompilationStruct>)Visit(parseTree);

    public override object? VisitStructDecl(WistGrammarParser.StructDeclContext context)
    {
        var name = context.IDENTIFIER(0).GetText();
        var fields = context.IDENTIFIER().Skip(1).Select(x => x.GetText()).ToArray();

        List<string> inheritances = new();
        if (context.inheritance() != null)
            inheritances = (List<string>)VisitInheritance(context.inheritance());

        _methods = new List<WistFuncName>();
        _curStructName = name;
        Visit(context.block());
        _curStructName = null;
        _list.Add(new WistCompilationStruct(name, fields, _methods.ToArray(), inheritances.ToArray()));
        _methods = null;
        return null;
    }

    public override object VisitInheritance(WistGrammarParser.InheritanceContext context)
    {
        return new List<string>(context.IDENTIFIER().Select(x => x.GetText()));
    }

    public override object? VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        var text = context.IDENTIFIER(0).GetText();
        var funcName = new WistFuncName(text, context.IDENTIFIER().Length - 1, _curStructName!);
        _methods?.Add(funcName);
        return null;
    }

    public override object Visit(IParseTree tree)
    {
        base.Visit(tree);
        return _list;
    }
}