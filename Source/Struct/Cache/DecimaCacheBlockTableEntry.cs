using System.Runtime.InteropServices;

namespace ZeroDawn.Struct.Cache
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct DecimaCacheBlockTableEntry
    {
        public long ChunkOffset;
        public int ChunkSize;
        public int Unknown1;
        public long Offset;
        public int Size;
        public int Unknown2;
    }
}
