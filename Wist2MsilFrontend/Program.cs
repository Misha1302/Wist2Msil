﻿using Antlr4.Runtime;
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
    foreach (var error in errors)
        PrintError(error.Message);

    return;
}


var compiler = new WistCompiler(visitor.Module);

#if RELEASE
try
{
#endif

var result = compiler.Run(out var compilationTime, out var executionTime);

Console.WriteLine($"Start function returned {result}");
Console.WriteLine($"Compilation took {compilationTime} ms");
Console.WriteLine($"Execution took {executionTime} ms");

#if RELEASE
}
catch (WistCompilerError e)
{
    var funcPath = "";
    if (e.FuncFullName.Owner != null)
        funcPath += $"Owner: {e.FuncFullName.Owner}\n";

    funcPath += $"Func name: {e.FuncFullName.Name}\n";
    funcPath += $"Args count: {e.FuncFullName.ArgsCount}";

    PrintError($"Compiler error in line: {e.Line}\n" + funcPath);
}
#endif


void PrintError(string s)
{
    var color = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;

    Console.WriteLine(s);

    Console.ForegroundColor = color;
}