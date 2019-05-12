using System.Runtime.InteropServices;

namespace ZeroDawn.Helper.DirectX
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DDSPixelFormat
    {
        public uint Size;
        public uint Flags;
        public uint FourCC;
        public uint BitCount;
        public uint RedMask;
        public uint GreenMask;
        public uint BlueMask;
        public uint AlphaMask;
    }
}
