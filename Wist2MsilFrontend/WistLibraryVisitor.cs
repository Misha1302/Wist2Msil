namespace Wist2MsilFrontend;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Wist2Msil;
using Wist2MsilFrontend.Content;
using WistConst;
using WistError;
using WistFastList;

public sealed class WistLibraryVisitor : WistGrammarBaseVisitor<object?>
{
    private static readonly HashSet<string> _visitedPaths = new();
    private WistLibraryManager? _libManager;
    private readonly string _path;
    private readonly WistFastList<WistFunction> _wistFunctions;
    private readonly WistFastList<WistCompilationStruct> _wistStructs;
    private readonly WistLibraryManager _wistLibraryManager;
    private readonly List<WistError> _lexerErrors;
    private readonly List<WistError> _parserErrors;

    public WistLibraryVisitor(string path, WistFastList<WistFunction> wistFunctions,
        WistFastList<WistCompilationStruct> wistStructs,
        WistLibraryManager wistLibraryManager, List<WistError> lexerErrors, List<WistError> parserErrors)
    {
        _path = path;
        _wistFunctions = wistFunctions;
        _wistStructs = wistStructs;
        _wistLibraryManager = wistLibraryManager;

        _lexerErrors = lexerErrors;
        _parserErrors = parserErrors;
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

            var grammarLexer = new WistGrammarLexer(
                new AntlrInputStream(
                    File.ReadAllText(fullPath)
                )
            );
            var grammarParser = new WistGrammarParser(
                new CommonTokenStream(
                    grammarLexer
                )
            );
            
            var parserErrorListener = new WistErrorListener($"{fullPath}. Parser error. ", _parserErrors);
            grammarParser.AddErrorListener(parserErrorListener);
            
            var lexerErrorListener = new WistErrorListener($"{fullPath}. Lexer error. ", _lexerErrors);
            grammarLexer.AddErrorListener(lexerErrorListener);
            
            
            var tree = grammarParser.program();

            var visitor = new WistVisitor(_path, _wistFunctions, _wistStructs, _wistLibraryManager, tree, _lexerErrors, _parserErrors);
            visitor.Visit(tree);
        }

        return null;
    }
}