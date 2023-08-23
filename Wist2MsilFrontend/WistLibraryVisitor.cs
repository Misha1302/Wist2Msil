namespace Wist2MsilFrontend;

using Antlr4.Runtime.Tree;
using Wist2MsilFrontend.Content;

public sealed class WistLibraryVisitor : WistGrammarBaseVisitor<object?>
{
    private WistLibraryManager? _libManager;

    public WistLibraryVisitor SetLibManager(WistLibraryManager? libraryManager)
    {
        _libManager = libraryManager;
        return this;
    }

    public override object? Visit(IParseTree tree)
    {
        if (_libManager is null)
            throw new InvalidOperationException();
        return base.Visit(tree);
    }

    public override object? VisitInclude(WistGrammarParser.IncludeContext context)
    {
        _libManager!.AddLibrary(context.STRING().GetText()[1..^1]);
        return null;
    }
}