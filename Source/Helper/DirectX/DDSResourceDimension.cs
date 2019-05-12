using System.Diagnostics.CodeAnalysis;

namespace ZeroDawn.Helper.DirectX
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum DDSResourceDimension : uint
    {
        UNKNOWN = 0,
        BUFFER = 1,
        TEXTURE1D = 2,
        TEXTURE2D = 3,
        TEXTURE3D = 4
    }
}
