namespace Wist2Msil;

using WistConst;

public static class WistProgram
{
    public static void Main()
    {
        var wistModule = new WistModule();
        var wistFunc = wistModule.MakeFunction("Start");
        var image = wistFunc.Image;

        image.PushConst(new WistConst(0));
        image.SetLocal("i");

        image.SetLabel("label");

        image.LoadLocal("i");
        image.PushConst(new WistConst(1));
        image.Add();
        image.SetLocal("i");

        // image.LoadLocal("i");
        // image.Call(typeof(WistProgram).GetMethod(nameof(PrintWistConst))!);
        // image.Drop();

        image.LoadLocal("i");
        image.PushConst(new WistConst(100_000_000));
        image.NegCmp();

        image.GotoIfTrue("label");

        var executionTimes = new List<long>();
        var compiler = new WistCompiler(wistModule);
        for (var i = 0; i < 10; i++)
        {
            compiler.Run(out var compilationTime, out var executionTime);
            executionTimes.Add(executionTime);
            Console.WriteLine($"exe time: {executionTime}; comp time: {compilationTime}");
        }

        Console.WriteLine(executionTimes.Average());
    }

    public static WistConst PrintWistConst(WistConst c)
    {
        Console.WriteLine(c);
        return default;
    }
}