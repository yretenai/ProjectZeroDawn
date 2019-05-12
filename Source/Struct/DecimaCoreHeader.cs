using System;
using System.Runtime.InteropServices;

namespace ZeroDawn.Struct
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct DecimaCoreHeader
    {
        public DecimaFileTypeMagic FileTypeMagic;
        public int DataSize;
        public fixed byte Checksum[0x10];

        public static implicit operator Guid(DecimaCoreHeader @this)
        {
            return new Guid(new ReadOnlySpan<byte>(@this.Checksum, 0x10));
        }

        public Guid GetChecksum()
        {
            return this;
        }

        public void SetChecksum(byte[] data)
        {
            fixed (byte* pin = Checksum)
            {
                data.CopyTo(new Span<byte>(pin, 0x10));
            }
        }
    }
}
