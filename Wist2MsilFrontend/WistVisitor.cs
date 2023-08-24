namespace Wist2MsilFrontend;

using Antlr4.Runtime.Tree;
using Wist2Msil;
using Wist2MsilFrontend.Content;
using WistConst;
using WistError;
using WistFuncName;

public sealed class WistVisitor : WistGrammarBaseVisitor<object?>
{
    private readonly WistModule _wistModule = new();
    private List<WistCompilationStruct> _wistStructs = null!;
    private List<WistFunction> _wistFunctions = null!;
    private WistFunction _curFunc = null!;
    private int _saveResultLevel;
    private bool _initialized;
    private string? _curStructName;
    private WistLibraryManager _wistLibraryManager = null!;
    private (string startLoopLabel, string endLoopLabel, string lastAssigmentLabel) _loopLabels;
    private readonly string _path;

    public WistVisitor(string path)
    {
        _path = path;
    }

    public WistVisitor(string path, List<WistFunction> wistFunctions, List<WistCompilationStruct> wistStructs,
        WistLibraryManager wistLibraryManager, IParseTree tree)
    {
        _path = path;
        wistFunctions.AddRange(new WistFunctionsVisitor().GetAllFunctions(tree));
        wistStructs.AddRange(new WistStructsVisitor().GetAllStructs(tree));
        new WistLibraryVisitor(path, wistFunctions, wistStructs, wistLibraryManager).SetLibManager(wistLibraryManager)
            .Visit(tree);

        _wistFunctions = wistFunctions;
        _wistStructs = wistStructs;
        _wistLibraryManager = wistLibraryManager;
        
        _initialized = true;
    }

    public override object? Visit(IParseTree tree)
    {
        if (!_initialized)
        {
            _initialized = true;
            _wistFunctions = new WistFunctionsVisitor().GetAllFunctions(tree);
            _wistStructs = new WistStructsVisitor().GetAllStructs(tree);
            _wistLibraryManager = new WistLibraryManager();
            new WistLibraryVisitor(_path, _wistFunctions, _wistStructs, _wistLibraryManager)
                .SetLibManager(_wistLibraryManager).Visit(tree);
        }

        base.Visit(tree);

        foreach (var wistStruct in _wistStructs)
            _wistModule.AddStruct(wistStruct);
        foreach (var wistFunction in _wistFunctions)
            _wistModule.AddFunction(wistFunction);

        return null;
    }

    public WistModule GetModule() => _wistModule;

    public override object? VisitVarAssigment(WistGrammarParser.VarAssigmentContext context)
    {
        var ident = context.IDENTIFIER().GetText();

        _saveResultLevel++;
        Visit(context.expression());
        _saveResultLevel--;

        _curFunc.Image.SetLocal(ident);

        return null;
    }

    public override object? VisitConstant(WistGrammarParser.ConstantContext context)
    {
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
            _curFunc.Image.PushConst(WistConst.CreateNull());
            return null;
        }

        throw new WistError("Missing type");
    }

    public override object? VisitIdentifierExpression(WistGrammarParser.IdentifierExpressionContext context)
    {
        var locName = context.IDENTIFIER().GetText();
        _curFunc.Image.LoadLocal(locName);
        return null;
    }

    public override object? VisitListExpression(WistGrammarParser.ListExpressionContext context)
    {
        _curFunc.Image.InstantiateList();
        foreach (var expression in context.expression())
        {
            _curFunc.Image.Dup();
            Visit(expression);
            _curFunc.Image.Call(typeof(WistBuildInFunctions).GetMethod(nameof(WistBuildInFunctions.AddToList)));
            _curFunc.Image.Drop();
        }

        return null;
    }

    public override object? VisitFuncCallExpression(WistGrammarParser.FuncCallExpressionContext context)
    {
        _saveResultLevel++;
        foreach (var expr in context.expression())
            Visit(expr);
        _saveResultLevel--;

        var text = context.IDENTIFIER().GetText();
        var fullName = WistFuncName.CreateFullName(text, context.expression().Length, null);
        var wistFunction = _wistFunctions.Find(x => x.Name.FullName == fullName);

        if (wistFunction != null)
            _curFunc.Image.Call(wistFunction);
        else
            _curFunc.Image.Call(_wistLibraryManager.GetMethod(fullName));

        if (_saveResultLevel == 0)
            _curFunc.Image.Drop();

        return null;
    }

    public override object? VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        var name = context.IDENTIFIER(0).GetText();
        var argsCount = context.IDENTIFIER().Length - 1;

        var wistFunction =
            _wistFunctions.Find(x => x.Name.FullName == WistFuncName.CreateFullName(name, argsCount, _curStructName))!;
        
        _curFunc = wistFunction;

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
        _saveResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _saveResultLevel--;

        if (context.ADD_OP().GetText() == "+")
            _curFunc.Image.Add();
        else
            _curFunc.Image.Sub();

        return null;
    }

    public override object? VisitMulExpression(WistGrammarParser.MulExpressionContext context)
    {
        _saveResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _saveResultLevel--;

        if (context.MUL_OP().GetText() == "*")
            _curFunc.Image.Mul();
        else
            _curFunc.Image.Div();

        return null;
    }

    public override object? VisitRemExpression(WistGrammarParser.RemExpressionContext context)
    {
        _saveResultLevel++;
        foreach (var expressionContext in context.expression())
            Visit(expressionContext);
        _saveResultLevel--;

        _curFunc.Image.Rem();

        return null;
    }

    public override object? VisitCmpExpression(WistGrammarParser.CmpExpressionContext context)
    {
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
        var wistStruct = _wistStructs.Find(x => x.Name == context.IDENTIFIER().GetText());
        if (wistStruct is null)
            throw new InvalidOperationException();

        if (_saveResultLevel != 0)
            _curFunc.Image.Instantiate(wistStruct);

        return null;
    }

    public override object? VisitStructDecl(WistGrammarParser.StructDeclContext context)
    {
        var wistStruct = _wistStructs.Find(x => x.Name == context.IDENTIFIER(0).GetText());
        if (wistStruct is null)
            throw new InvalidOperationException();

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

        for (var index = 1; index < context.expression().Length; index++)
            Visit(context.expression(index));

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

        Visit(context.expression());
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

        Visit(context.expression());
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
        Visit(context.expression(1));
        _saveResultLevel--;
        _curFunc.Image.SetField(context.IDENTIFIER().GetText());
        return null;
    }
}