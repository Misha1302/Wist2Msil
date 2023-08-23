namespace Wist2Msil;

public static class WistProgram
{
    public static void Main()
    {
        Console.WriteLine("Obsolete");
        /*
        var wistModule = new WistModule();
        var start = wistModule.MakeFunction("Start");
        var init = wistModule.MakeFunction("init", new[] { "exeHelper", "struct" });
        var mStruct = wistModule.MakeStruct("mStruct", new[] { "a", "b", "c" }, new[] { "init" });

        start.Image.Instantiate(mStruct);
        start.Image.SetLocal("struct");

        start.Image.LoadLocal("struct"); // method owner
        start.Image.LoadLocal("struct"); // method arg
        start.Image.Call("init", 1);
        start.Image.Drop();

        start.Image.LoadLocal("struct");
        start.Image.PushField("b");
        start.Image.LoadLocal("struct");
        start.Image.PushField("a");
        start.Image.Mul();
        start.Image.Call(typeof(BuildInFunctions).GetMethod(nameof(BuildInFunctions.Print))!);
        start.Image.Drop();

        // start.Image.Call(typeof(WistProgram).GetMethod(nameof(PrintWistConst))!);
        // start.Image.Call(typeof(WistProgram).GetMethod(nameof(InputString))!);

        init.Image.LoadArg("struct");
        init.Image.PushConst(new WistConst(5));
        init.Image.SetField("a");

        init.Image.LoadArg("struct");
        init.Image.PushConst(new WistConst(-6));
        init.Image.SetField("b");


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
#pragma warning restore CS0162*/
    }
}