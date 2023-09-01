namespace WistConst;

public enum WistType : long
{
    None = 0,
    Null = 1 << 0,
    Number = 1 << 1,
    String = 1 << 2,
    Bool = 1 << 3,
    List = 1 << 4,
    InternalInteger = 1 << 5,
    Pointer = 1 << 6,
    Struct = 1 << 7,
    StructInternal = 1 << 8,
    MInfo = 1 << 9,
    RepeatEnumerator = 1 << 10,
    ValueType = Number | Bool | Pointer,
}