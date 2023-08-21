namespace Wist2MsilFrontend;

using System.Reflection;
using Antlr4.Runtime.Tree;
using Wist2Msil;
using Wist2MsilFrontend.Content;
using WistConst;
using WistError;

public sealed class WistVisitor : WistGrammarBaseVisitor<object?>
{
    private readonly WistModule _wistModule = new();
    private List<WistFunction> _wistFunctions = null!;
    private WistFunction _curFunc = null!;
    private int _saveResultLevel;
    private bool _initialized;

    public override object? Visit(IParseTree tree)
    {
        if (!_initialized)
        {
            _initialized = true;
            _wistFunctions = new WistFunctionsVisitor().GetAllFunctions(tree);
        }

        base.Visit(tree);
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
        _curFunc.Image.LoadLocal(context.IDENTIFIER().GetText());
        return null;
    }

    public override object? VisitFuncCall(WistGrammarParser.FuncCallContext context)
    {
        _saveResultLevel++;
        foreach (var expr in context.expression())
            Visit(expr);
        _saveResultLevel--;

        var text = context.IDENTIFIER().GetText();
        var wistFunction = _wistFunctions.Find(x => x.Name == text);

        if (wistFunction != null)
            _curFunc.Image.Call(wistFunction);
        else
            _curFunc.Image.Call(typeof(BuildInFunctions).GetMethod(text, BindingFlags.Public | BindingFlags.Static));

        if (_saveResultLevel == 0)
            _curFunc.Image.Drop();

        return null;
    }

    public override object? VisitFuncDecl(WistGrammarParser.FuncDeclContext context)
    {
        var name = context.IDENTIFIER(0).GetText();
        var wistFunction = _wistFunctions.Find(x => x.Name == name)!;
        _wistModule.AddFunction(wistFunction);
        _curFunc = wistFunction;

        Visit(context.block());

        return null;
    }

    public override object? VisitRet(WistGrammarParser.RetContext context)
    {
        Visit(context.expression());
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

        var elseLabelName = GenerateElseLabelName();
        var endElseLabelName = GenerateEndElseLabelName();
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

    private static string GenerateElseLabelName() => $"else_{Guid.NewGuid()}";
    private static string GenerateEndElseLabelName() => $"end_else_{Guid.NewGuid()}";
}