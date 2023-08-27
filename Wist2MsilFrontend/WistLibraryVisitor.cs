namespace Wist2MsilFrontend;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Wist2Msil;
using Wist2MsilFrontend.Content;
using WistConst;

public sealed class WistLibraryVisitor : WistGrammarBaseVisitor<object?>
{
    private static readonly HashSet<string> _visitedPaths = new();
    private WistLibraryManager? _libManager;
    private readonly string _path;
    private readonly List<WistFunction> _wistFunctions;
    private readonly List<WistCompilationStruct> _wistStructs;
    private readonly WistLibraryManager _wistLibraryManager;

    public WistLibraryVisitor(string path, List<WistFunction> wistFunctions, List<WistCompilationStruct> wistStructs,
        WistLibraryManager wistLibraryManager)
    {
        _path = path;
        _wistFunctions = wistFunctions;
        _wistStructs = wistStructs;
        _wistLibraryManager = wistLibraryManager;
    }

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
        var path = context.STRING().GetText()[1..^1];
        if (path.EndsWith("dll"))
        {
            _libManager!.AddLibrary(path);
        }
        else
        {
            var fullPath = Path.GetFullPath(Path.Combine(_path, path));
            if (!File.Exists(fullPath))
                fullPath = path;

            if (_visitedPaths.Contains(fullPath))
                return null;

            _visitedPaths.Add(fullPath);

            var tree = new WistGrammarParser(
                new CommonTokenStream(
                    new WistGrammarLexer(
                        new AntlrInputStream(
                            File.ReadAllText(fullPath)
                        )
                    )
                )
            ).program();

            var visitor = new WistVisitor(_path, _wistFunctions, _wistStructs, _wistLibraryManager, tree);
            visitor.Visit(tree);
        }

        return null;
    }
}