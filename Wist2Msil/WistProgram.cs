namespace Wist2Msil;

using WistConst;

public static class WistProgram
{
    public static void Main()
    {
        var wistModule = new WistModule();
        var start = wistModule.MakeFunction("Start");

        start.Image.PushConst(new WistConst("qwerty"));
        start.Image.PushConst(new WistConst("uiop"));
        start.Image.Add();
        start.Image.PushConst(new WistConst("qqq"));
        start.Image.Add();

        start.Image.Dup();
        start.Image.Call(typeof(WistProgram).GetMethod(nameof(PrintWistConst))!);
        start.Image.Drop();

        start.Image.Call(typeof(WistProgram).GetMethod(nameof(InputString))!);
        start.Image.Sub();
        start.Image.Call(typeof(WistProgram).GetMethod(nameof(PrintWistConst))!);
        start.Image.Drop();

        var executionTimes = new List<long>();
        var compiler = new WistCompiler(wistModule);
        const int repeatCount = 1;
        for (var i = 0; i < repeatCount; i++)
        {
            compiler.Run(out var compilationTime, out var executionTime);
            executionTimes.Add(executionTime);
            Console.WriteLine($"exe time: {executionTime}; comp time: {compilationTime}");
        }

        // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable CS0162
        if (repeatCount > 1)
            Console.WriteLine($"average exe time: {executionTimes.Average()}");
#pragma warning restore CS0162
    }

    public static WistConst PrintWistConst(WistConst c)
    {
        Console.WriteLine(c);
        return default;
    }

    public static WistConst InputString() => new(Console.ReadLine() ?? "\n");
}