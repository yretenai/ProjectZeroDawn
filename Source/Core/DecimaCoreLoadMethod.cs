namespace ZeroDawn.Core
{
    public enum DecimaCoreLoadMethod : byte
    {
        NotPresent = 0x00,
        Embedded = 0x01,
        ImmediateCoreFile = 0x02,
        CoreFile = 0x03,
        WorkOnly = 0x05
    }
}
