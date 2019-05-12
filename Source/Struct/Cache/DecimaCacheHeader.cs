using System.Runtime.InteropServices;

namespace ZeroDawn.Struct.Cache
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct DecimaCacheHeader
    {
        public uint MagicBytes;
        public int LoadOrder;
        public long CompressedSize;
        public long UncompressedSize;
        public long RecordTableSize;
        public int BlockTableSize;
        public int BlockSize;
    }
}
