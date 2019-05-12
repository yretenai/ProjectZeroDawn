using System.Runtime.InteropServices;

namespace ZeroDawn.Helper.DirectX
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct DDSImageHeader
    {
        public uint Magic;
        public uint Size;
        public uint Flags;
        public uint Height;
        public uint Width;
        public uint LinearSize;
        public uint Depth;
        public uint MipmapCount;
        public fixed uint Reserved1[11];
        public DDSPixelFormat Format;
        public uint Caps1;
        public uint Caps2;
        public uint Caps3;
        public uint Caps4;
        public uint Reserved2;
        public uint DXGIFormat;
        public DDSResourceDimension Dimension;
        public uint Misc; // cube = 0x4
        public uint MapSize; // number of maps, 1
        public uint Misc2; // alpha mode, 0
    }
}
