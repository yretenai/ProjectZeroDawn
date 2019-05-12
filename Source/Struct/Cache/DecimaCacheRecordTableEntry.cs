using System.Runtime.InteropServices;

namespace ZeroDawn.Struct.Cache
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct DecimaCacheRecordTableEntry
    {
        public int FileId;
        public int Unknown1;
        public long Hash;
        public long Offset;
        public int Size;
        public int Unknown2;
    }
}
