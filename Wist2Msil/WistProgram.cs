namespace Wist2Msil;

using WistConst;

public static class WistProgram
{
    public static void Main()
    {
        var wistModule = new WistModule();
        var start = wistModule.MakeFunction("Start");
        var cube = wistModule.MakeFunction("Cube", new[] { "n" });

        start.Image.PushConst(new WistConst(0));
        start.Image.SetLocal("i");
        start.Image.SetLabel("loopStart");

        start.Image.LoadLocal("i");
        start.Image.PushConst(new WistConst(50_000_000));
        start.Image.LessThan();
        start.Image.GotoIfFalse("loopEnd");

        start.Image.LoadLocal("i");
        start.Image.Call(cube);
        // start.Image.Call(typeof(WistProgram).GetMethod(nameof(PrintWistConst))!);
        start.Image.Drop();

        start.Image.LoadLocal("i");
        start.Image.PushConst(new WistConst(1));
        start.Image.Add();
        start.Image.SetLocal("i");
        start.Image.Goto("loopStart");

        start.Image.SetLabel("loopEnd");


        cube.Image.LoadArg("n");
        cube.Image.LoadArg("n");
        cube.Image.LoadArg("n");
        cube.Image.Mul();
        cube.Image.Mul();
        cube.Image.Ret();

        var executionTimes = new List<long>();
        var compiler = new WistCompiler(wistModule);
        for (var i = 0; i < 10; i++)
        {
            compiler.Run(out var compilationTime, out var executionTime);
            executionTimes.Add(executionTime);
            Console.WriteLine($"exe time: {executionTime}; comp time: {compilationTime}");
        }

        Console.WriteLine($"average exe time: {executionTimes.Average()}");
    }

    public static WistConst PrintWistConst(WistConst c)
    {
        Console.WriteLine(c);
        return default;
    }
}