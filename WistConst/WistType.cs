namespace WistConst;

public enum WistType : long
{
    None = 1 << 0,
    Null = 1 << 1,
    Number = 1 << 2,
    String = 1 << 3,
    Bool = 1 << 4,
    List = 1 << 5,
    InternalInteger = 1 << 6,
    Pointer = 1 << 7,
    Struct = 1 << 8,
    StructInternal = 1 << 9,
    MInfo = 1 << 10,
    ValueType = Number | Bool | Pointer
}