namespace Wist2MsilFrontend;

public static class WistLabelsManager
{
    public static string ElseStartLabelName() => LabelName("else_start");
    public static string ElseEndLabelName() => LabelName("else_end");

    public static string WhileStartLabelName() => LabelName("while_start");
    public static string WhileEndLabelName() => LabelName("while_end");

    public static string ForStartLabelName() => LabelName("for_start");
    public static string ForEndLabelName() => LabelName("for_end");
    public static string ForLastAssigmentLabelName() => LabelName("for_last_assigment");

    public static string LabelName(string prefix) => $"{prefix}_{Guid.NewGuid()}";
}