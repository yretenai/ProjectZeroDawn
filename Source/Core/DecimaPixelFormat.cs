using System.Diagnostics.CodeAnalysis;

namespace ZeroDawn.Core
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum DecimaPixelFormat : byte
    {
        R8G8B8A8 = 0x0C,
        A8 = 0x1F,
        BC1 = 0x42,
        BC2 = 0x43,
        BC3 = 0x44,
        BC4 = 0x45,
        BC5 = 0x47,
        BC7 = 0x4B
    }
}
