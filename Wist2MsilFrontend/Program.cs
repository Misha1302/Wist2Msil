using Antlr4.Runtime;
using Wist2Msil;
using Wist2MsilFrontend;
using Wist2MsilFrontend.Content;

const string dir = @"Content\Code";
const string path = @$"{dir}\Code.wist";

var code = File.ReadAllText(path);

var inputStream = new AntlrInputStream(code);
var simpleLexer = new WistGrammarLexer(inputStream);
// simpleLexer.AddErrorListener(new WistThrowingErrorListener());

var commonTokenStream = new CommonTokenStream(simpleLexer);
var simpleParser = new WistGrammarParser(commonTokenStream);
// simpleParser.AddErrorListener(new WistThrowingErrorListener());

var program = simpleParser.program();
var visitor = new WistVisitor(dir);
visitor.Visit(program);

var module = visitor.GetModule();
var compiler = new WistCompiler(module);
var result = compiler.Run(out var compilationTime, out var executionTime);
Console.WriteLine($"Start function returned {result}");
Console.WriteLine($"Compilation took {compilationTime} ms");
Console.WriteLine($"Execution took {executionTime} ms");