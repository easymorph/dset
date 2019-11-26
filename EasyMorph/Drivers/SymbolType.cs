namespace EasyMorph.Drivers
{
    enum SymbolType : byte
    {
        Nothing = 0,
        Int8 = 1,
        Int16 = 2,
        Int32 = 4,
        Decimal = 16,
        Text = 32,
        BoolTrue = 64,
        BoolFalse = 65,
        Error = 128
    }
}