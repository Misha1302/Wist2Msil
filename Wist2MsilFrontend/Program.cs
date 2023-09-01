using Antlr4.Runtime;
using Wist2Msil;
using Wist2MsilFrontend;
using Wist2MsilFrontend.Content;

const string dir = @"Content\Code";
const string path = @$"{dir}\Code.wist";

var code = File.ReadAllText(path);

var inputStream = new AntlrInputStream(code);
var simpleLexer = new WistGrammarLexer(inputStream);
var lexerErrors = new List<WistError.WistError>();
simpleLexer.AddErrorListener(new WistErrorListener($"{path}. Lexer error. ", lexerErrors));

var commonTokenStream = new CommonTokenStream(simpleLexer);
var simpleParser = new WistGrammarParser(commonTokenStream);
var parserErrors = new List<WistError.WistError>();
simpleParser.AddErrorListener(new WistErrorListener($"{path}. Parser error. ", parserErrors));

var program = simpleParser.program();
var visitor = new WistVisitor(dir, lexerErrors, parserErrors);
visitor.Visit(program);


var errors = lexerErrors.Concat(parserErrors).ToList();
if (errors.Count != 0)
{
    var color = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;

    foreach (var error in errors)
        Console.WriteLine(error.Message);

    Console.ForegroundColor = color;

    return;
}


var compiler = new WistCompiler(visitor.WistModule);
var result = compiler.Run(out var compilationTime, out var executionTime);
Console.WriteLine($"Start function returned {result}");
Console.WriteLine($"Compilation took {compilationTime} ms");
Console.WriteLine($"Execution took {executionTime} ms");