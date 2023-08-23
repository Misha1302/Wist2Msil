namespace WistFuncName;

public sealed record WistFuncName(string Name, int ArgsCount, string? Owner)
{
    public string FullName =>
        !string.IsNullOrWhiteSpace(Owner)
            ? $"{Name}{ArgsCount}<>{Owner}"
            : $"{Name}{ArgsCount}";

    public string NameWithoutOwner => $"{Name}{ArgsCount}";

    public static string CreateFullName(string name, int argsCount, string? owner) =>
        !string.IsNullOrWhiteSpace(owner)
            ? $"{name}{argsCount}<>{owner}"
            : $"{name}{argsCount}";
}