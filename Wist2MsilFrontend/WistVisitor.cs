﻿namespace Wist2MsilFrontend;

using Antlr4.Runtime.Tree;
using Wist2Msil;
using Wist2MsilFrontend.Content;
using WistConst;
using WistError;
using WistFastList;
using WistFuncName;

public sealed class WistVisitor : WistGrammarBaseVisitor<object?>
{
    private WistFastList<WistCompilationStruct> _wistStructs = null!;
    private WistFastList<WistFunction> _wistFunctions = null!;
    private WistFunction _curFunc = null!;
    private int _saveResultLevel;
    private bool _initialized;
    private string? _curStructName;
    private WistLibraryManager _wistLibraryManager = null!;
    private (string startLoopLabel, string endLoopLabel, string lastAssigmentLabel) _loopLabels;
    private readonly string _path;
    private readonly List<WistError> _lexerErrors;
    private readonly List<WistError> _parserErrors;
    private int _curLine = 1;

    public WistVisitor(string path, List<WistError> lexerErrors,
        List<WistError> parserErrors)
    {
        _path = path;
        _lexerErrors = lexerErrors;
        _parserErrors = parserErrors;
    }

    public WistVisitor(string path, WistFastList<WistFunction> wistFunctions,
        WistFastList<WistCompilationStruct> wistStructs,
        WistLibraryManager wistLibraryManager, IParseTree tree, List<WistError> lexerErrors,
        List<WistError> parserErrors)
    {
        _path = path;
        wistFunctions.AddRange(new WistFunctionsVisitor().GetAllFunctions(tree));
        wistStructs.AddRange(new WistStructsVisitor().GetAllStructs(tree));
        new WistLibraryVisitor(path, wistFunctions, wistStructs, wistLibraryManager, lexerErrors, parserErrors)
            .SetLibManager(wistLibraryManager)
            .Visit(tree);

        _wistFunctions = wistFunctions;
        _wistStructs = wistStructs;
        _wistLibraryManager = wistLibraryManager;
        _lexerErrors = lexerErrors;
        _parserErrors = parserErrors;

        _initialized = true;
    }

    public WistModule Module { get; } = new();

    public override object? Visit(IParseTree tree)
    {
        try
        {
            if (!_initialized)
            {
                _initialized = true;
                _wistFunctions = new WistFunctionsVisitor().GetAllFunctions(tree);
                _wistStructs = new WistStructsVisitor().GetAllStructs(tree);
                _wistLibraryManager = new WistLibraryManager();
                new WistLibraryVisitor(_path, _wistFunctions, _wistStructs, _wistLibraryManager, _lexerErrors,
                        _parserErrors)
                    .SetLibManager(_wistLibraryManager).Visit(tree);
            }

            base.Visit(tree);
        }
        catch
        {
            // ignored
        }
        finally
        {
            foreach (var wistStruct in _wistStructs)
                Module.AddStruct(wistStruct);
            foreach (var wistFunction in _wistFunctions)
                Module.AddFunction(wistFunction);
        }

        return null;
    }

    public override object? VisitRepeatLoop(WistGrammarParser.RepeatLoopContext context)
    {
        var methodInfo = typeof(WistVisitorHelper).GetMethod(nameof(WistVisitorHelper.InstantiateRepeatEnumerator));

        var expressionContexts = context.expression();


        if (expressionContexts.Length == 1)
            _curFunc.Image.PushConst(new WistConst(1));

        _saveResultLevel++;
        foreach (var expressionContext in expressionContexts)
            Visit(expressionContext);
        _saveResultLevel--;

        if (expressionContexts.Length <= 2)
            _curFunc.Image.PushConst(new WistConst(1));

        _curFunc.Image.Call(methodInfo);

        var startLabelName = WistLabelsManager.RepeatStartLabelName();
        var endLabelName = WistLabelsManager.RepeatEndLabelName();
        _loopLabels = (startLabelName, endLabelName, string.Empty);

        _curFunc.Image.SetLabel(startLabelName);
        _curFunc.Image.Dup();
        _curFunc.Image.GotoIfNext(endLabelName);

        _curFunc.Image.Dup();
        _curFunc.Image.Current();
        _curFunc.Image.SetLocal(context.IDENTIFIER().GetText());

        Visit(context.block());
        _curFunc.Image.Goto(startLabelName);

        _curFunc.Image.SetLabel(endLabelName);

        _curFunc.Image.Drop();

        return null;
    }


    public override object? VisitEndOfLine(WistGrammarParser.EndOfLineContext context)
    {
        if (context.END_OF_TEXT_LINE() != null)
            _curLine++;
        return null;
    }

    public override object? VisitVarAssigment(WistGrammarParser.VarAssigmentContext context)
    {
        var text = context.ASSIGMENT_SIGN().GetText();
        var varName = context.IDENTIFIER().GetText();


        if (text.Length != 1)
            _curFunc.Image.LoadLocal(varName);


        _saveResultLevel++;
        Visit(context.expression());
        _saveResultLevel--;


        if (text.Length != 1) EmitMathOp(text[..1]);

        _curFunc.Image.SetLocal(varName);

        return null;
    }

    public override object? VisitConstant(WistGrammarParser.ConstantContext context)
    {
        if (_saveResultLevel == 0) return null;

        if (context.NUMBER() != null)
        {
            var value = context.NUMBER().GetText().ToDouble();
            _curFunc.Image.PushConst(new WistConst(value));
            return value;
        }

        if (context.STRING() != null)
        {
            var value = context.STRING().GetText()[1..^1];
            _curFunc.Image.PushConst(new WistConst(value));
            return value;
        }

        if (context.BOOL() != null)
        {
            var value = context.BOOL().GetText().ToBool();
            _curFunc.Image.PushConst(new WistConst(value));
            return value;
        }

        if (context.NULL() != null)
        {
            _curFunc.Image.PushConst(context.NULL().GetText() == "none" ? default : WistConst.CreateNull());
            return null;
        }

        throw new WistError("Missing type");
    }

    public override object? VisitIdentifierExpression(WistGrammarParser.IdentifierExpressionContext context)
    {
        if (_saveResultLevel == 0) return null;

        var locName = context.IDENTIFIER().GetText();

        var firstOrDefault = _wistFunctions.FirstOrDefault(x => x.Name.FullName == locName);
        if (firstOrDefault != null)
        {
            _curFunc.Image.InstantiateFunctionPtr(firstOrDefault);
            return null;
        }

        _curFunc.Image.LoadLocal(locName);
        return null;
    }

    public override object? VisitListExpression(WistGrammarParser.ListExpressionContext context)
    {
        if (_saveResultLevel == 0) return null;

        var expressionContexts = context.expression();
        foreach (var expression in expressionContexts)
            Visit(expression);

        _curFunc.Image.InstantiateList(expressionContexts.Length);

        return null;
    }

    public override object? VisitFuncCallExpression(WistGrammarParser.FuncCallExpressionContext context)
    {
        var expressions = context.expression();
        _saveResultLevel++;
        foreach (var expr in expressions)
            Visit(expr);
        _saveResultLevel--;

        var text = context.IDENTIFIER().GetText();
        var fullName = WistFuncName.CreateFullName(text, expressions.Length, null);
        var wistFunction = _wistFunctions.FirstOrDefault(x => x.Name.FullName == fullName);

        if (wistFunction != null)
        {
            _curFunc.Image.Call(wistFunction);
        }
        else
        {
            var methodInfo = _wistLibraryManager.GetMethod(fullName);
            if (methodInfo != null)
            {
                _curFunc.Image.Call(methodInfo);
            }
            else
            {
                _curFunc.Image.LoadLocal(text);
                _curFunc.Image.CallVariable(expressions.Length);
            }
        }

        if (_saveResultLevel == 0)
            _curFunc.Image.Drop();

        return null;
    }

    public override object? VisitExpressionCallExpression(WistGrammarParser.ExpressionCallExpressionContext context)
    {
        var locTempName = Guid.NewGuid().ToString();

        _saveResultLevel++;
        Visit(context.expression(0));
        _curFunc.Image.SetLocal(locTempName);

        var expressions = context.expression().Skip(1).ToArray();
        foreach (var expr in expressions)
            Visit(expr);

        _saveResultLevel--;
        _curFunc.Image.LoadLocal(locTempName);
        _curFunc.Image.CallVariable(expressions.Length);

        if (_saveResultLevel == 0)
            _curFunc.Image.Drop();

        return null;
    }

    public override object? VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        var name = context.IDENTIFIER(0).GetText();
        var argsCount = context.IDENTIFIER().Length - 1;

        var wistFunction =
            _wistFunctions.FirstOrDefault(x =>
                x.Name.FullName == WistFuncName.CreateFullName(name, argsCount, _curStructName))!;

        _curFunc = wistFunction;
        _curFunc.Image.InitGetLineAction(() => _curLine);

        Visit(context.block());

        return null;
    }

    public override object? VisitRet(WistGrammarParser.RetContext context)
    {
        _saveResultLevel++;
        Visit(context.expression());
        _saveResultLevel--;
        _curFunc.Image.Ret();

        return null;
    }

    public override object? VisitAddExpression(WistGrammarParser.AddExpressionContext context)
    {
        if (_saveResultLevel == 0) return null;

        _saveResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _saveResultLevel--;

        EmitMathOp(context.ADD_OP().GetText());

        return null;
    }

    private void EmitMathOp(string sign)
    {
        Action act = sign switch
        {
            "+" => _curFunc.Image.Add,
            "-" => _curFunc.Image.Sub,
            "*" => _curFunc.Image.Mul,
            "/" => _curFunc.Image.Div,
            "%" => _curFunc.Image.Rem,
            "^" => _curFunc.Image.Pow,
            _ => throw new ArgumentOutOfRangeException(nameof(sign), sign, null)
        };

        act();
    }

    public override object? VisitMulExpression(WistGrammarParser.MulExpressionContext context)
    {
        if (_saveResultLevel == 0) return null;

        _saveResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _saveResultLevel--;

        EmitMathOp(context.MUL_OP().GetText());

        return null;
    }

    public override object? VisitRemExpression(WistGrammarParser.RemExpressionContext context)
    {
        if (_saveResultLevel == 0) return null;

        _saveResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _saveResultLevel--;

        EmitMathOp(context.REM_OP().GetText());

        return null;
    }

    public override object? VisitPowExpression(WistGrammarParser.PowExpressionContext context)
    {
        if (_saveResultLevel == 0) return null;

        _saveResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _saveResultLevel--;

        EmitMathOp(context.POW_OP().GetText());

        return null;
    }

    public override object? VisitCmpExpression(WistGrammarParser.CmpExpressionContext context)
    {
        if (_saveResultLevel == 0) return null;

        _saveResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _saveResultLevel--;

        Action act = context.CMP_OP().GetText() switch
        {
            "==" => _curFunc.Image.Cmp,
            "!=" => _curFunc.Image.NegCmp,
            ">" => _curFunc.Image.GreaterThan,
            "<" => _curFunc.Image.LessThan,
            "<=" => _curFunc.Image.LessThanOrEquals,
            ">=" => _curFunc.Image.GreaterThanOrEquals,
            _ => throw new ArgumentOutOfRangeException(context.CMP_OP().GetText())
        };

        act();

        return null;
    }

    public override object? VisitLabelDecl(WistGrammarParser.LabelDeclContext context)
    {
        _curFunc.Image.SetLabel(context.IDENTIFIER().GetText());
        return null;
    }

    public override object? VisitJmp(WistGrammarParser.JmpContext context)
    {
        _curFunc.Image.Goto(context.IDENTIFIER().GetText());
        return null;
    }

    public override object? VisitIfBlock(WistGrammarParser.IfBlockContext context)
    {
        _saveResultLevel++;
        Visit(context.expression());
        _saveResultLevel--;

        var elseLabelName = WistLabelsManager.ElseStartLabelName();
        var endElseLabelName = WistLabelsManager.ElseEndLabelName();
        _curFunc.Image.GotoIfFalse(elseLabelName);

        // if block
        Visit(context.block());

        _curFunc.Image.Goto(endElseLabelName);
        _curFunc.Image.SetLabel(elseLabelName);

        // else block
        if (context.elseBlock() != null)
            Visit(context.elseBlock());

        _curFunc.Image.SetLabel(endElseLabelName);

        return null;
    }

    public override object? VisitNewStruct(WistGrammarParser.NewStructContext context)
    {
        var wistStruct = _wistStructs.FirstOrDefault(x => x.Name == context.IDENTIFIER().GetText());

        if (wistStruct is null)
        {
            _parserErrors.Add(new WistError($"Line: {_curLine}. {context.IDENTIFIER().GetText()} struct wasn't found"));

            throw _parserErrors[^1];
        }

        var argsCount = context.expression().Length + 1;

        var fullName = WistFuncName.CreateFullName("Constructor", argsCount, null);
        var ctorFound = wistStruct.Methods.Any(x => x.NameWithoutOwner == fullName);

        if (!ctorFound && _saveResultLevel == 0)
            return null;

        _curFunc.Image.Instantiate(wistStruct);

        if (!ctorFound)
        {
            if (argsCount == 1)
                return null;

            _parserErrors.Add(new WistError($"Line: {_curLine}. Constructor with {argsCount} args wasn't found"));
            throw _parserErrors[^1];
        }

        _curFunc.Image.Dup();

        _saveResultLevel++;
        for (var index = 0; index < context.expression().Length; index++)
            Visit(context.expression(index));
        _saveResultLevel--;

        _curFunc.Image.Call(fullName, argsCount - 1);

        if (_saveResultLevel == 0)
            _curFunc.Image.Drop();


        return null;
    }

    public override object? VisitStructDecl(WistGrammarParser.StructDeclContext context)
    {
        var wistStruct = _wistStructs.FirstOrDefault(x => x.Name == context.IDENTIFIER(0).GetText());
        if (wistStruct is null)
        {
            _parserErrors.Add(new WistError(
                $"Line: {_curLine}. Struct with name {context.IDENTIFIER(0).GetText()} wasn't found"
            ));
            throw _parserErrors[^1];
        }

        _curStructName = wistStruct.Name;
        Visit(context.block());
        _curStructName = null;

        return null;
    }

    public override object? VisitStructFuncCallExpression(WistGrammarParser.StructFuncCallExpressionContext context)
    {
        _saveResultLevel++;
        Visit(context.expression(0));
        _curFunc.Image.Dup();
        _saveResultLevel--;

        _saveResultLevel++;
        for (var index = 1; index < context.expression().Length; index++)
            Visit(context.expression(index));
        _saveResultLevel--;

        var argsCount = context.expression().Length;
        var fullName = WistFuncName.CreateFullName(context.IDENTIFIER().GetText(), argsCount, null);
        _curFunc.Image.Call(fullName, argsCount - 1);

        if (_saveResultLevel == 0)
            _curFunc.Image.Drop();

        return null;
    }

    public override object? VisitBreak(WistGrammarParser.BreakContext context)
    {
        _curFunc.Image.Goto(_loopLabels.endLoopLabel);
        return null;
    }

    public override object? VisitContinue(WistGrammarParser.ContinueContext context)
    {
        _curFunc.Image.Goto(
            string.IsNullOrEmpty(_loopLabels.lastAssigmentLabel)
                ? _loopLabels.startLoopLabel
                : _loopLabels.lastAssigmentLabel
        );
        return null;
    }

    public override object? VisitWhileLoop(WistGrammarParser.WhileLoopContext context)
    {
        var whileStartLabelName = WistLabelsManager.WhileStartLabelName();
        var whileEndLabelName = WistLabelsManager.WhileEndLabelName();

        _loopLabels = (whileStartLabelName, whileEndLabelName, string.Empty);

        _curFunc.Image.SetLabel(whileStartLabelName);

        _saveResultLevel++;
        Visit(context.expression());
        _saveResultLevel--;
        _curFunc.Image.GotoIfFalse(whileEndLabelName);

        Visit(context.block());

        _curFunc.Image.Goto(whileStartLabelName);
        _curFunc.Image.SetLabel(whileEndLabelName);

        return null;
    }

    public override object? VisitForLoop(WistGrammarParser.ForLoopContext context)
    {
        var forStartLabelName = WistLabelsManager.ForStartLabelName();
        var forEndLabelName = WistLabelsManager.ForEndLabelName();
        var forLastAssigmentLabelName = WistLabelsManager.ForLastAssigmentLabelName();

        _loopLabels = (forStartLabelName, forEndLabelName, forLastAssigmentLabelName);

        Visit(context.assigment(0));

        _curFunc.Image.SetLabel(forStartLabelName);

        _saveResultLevel++;
        Visit(context.expression());
        _saveResultLevel--;
        _curFunc.Image.GotoIfFalse(forEndLabelName);

        Visit(context.block());


        _curFunc.Image.SetLabel(forLastAssigmentLabelName);
        Visit(context.assigment(1));

        _curFunc.Image.Goto(forStartLabelName);
        _curFunc.Image.SetLabel(forEndLabelName);

        return null;
    }

    public override object? VisitStructFieldExpression(WistGrammarParser.StructFieldExpressionContext context)
    {
        if (_saveResultLevel == 0) return null;

        _saveResultLevel++;
        Visit(context.expression());
        _saveResultLevel--;
        _curFunc.Image.PushField(context.IDENTIFIER().GetText());
        return null;
    }

    public override object? VisitStructFieldAssigment(WistGrammarParser.StructFieldAssigmentContext context)
    {
        _saveResultLevel++;
        Visit(context.expression(0));
        _saveResultLevel--;

        var text = context.ASSIGMENT_SIGN().GetText();
        var fieldName = context.IDENTIFIER().GetText();

        if (text.Length != 1)
        {
            _curFunc.Image.Dup();
            _curFunc.Image.PushField(fieldName);

            _saveResultLevel++;
            Visit(context.expression(1));
            _saveResultLevel--;
            EmitMathOp(text[..1]);
            _curFunc.Image.SetField(fieldName);
        }
        else
        {
            _saveResultLevel++;
            Visit(context.expression(1));
            _saveResultLevel--;
            _curFunc.Image.SetField(fieldName);
        }


        return null;
    }
}