using System;

namespace ZeroDawn.Helper.DirectX
{
    public class DXGIPixelFormatInfoAttribute : Attribute
    {
        public readonly int BitsPerPixel;
        public readonly int BlockSize = 1;

        public DXGIPixelFormatInfoAttribute(int bpp)
        {
            BitsPerPixel = bpp;
        }

        public DXGIPixelFormatInfoAttribute(int bpp, int blockSize)
        {
            BitsPerPixel = bpp;
            BlockSize = blockSize;
        }

        public override object TypeId => this;

        public override bool Match(object obj)
        {
            if (obj is DXGIPixelFormatInfoAttribute attr)
                return attr.BitsPerPixel == BitsPerPixel && attr.BlockSize == BlockSize;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is DXGIPixelFormatInfoAttribute attr)
                return attr.BitsPerPixel == BitsPerPixel && attr.BlockSize == BlockSize;

            return false;
        }

        protected bool Equals(DXGIPixelFormatInfoAttribute other)
        {
            return base.Equals(other) && other.BitsPerPixel == BitsPerPixel && other.BlockSize == BlockSize;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ BitsPerPixel;
                hashCode = (hashCode * 397) ^ BlockSize;
                return hashCode;
            }
        }

        public override bool IsDefaultAttribute()
        {
            return BitsPerPixel == default;
        }

        public (int bpp, int block) Unpack()
        {
            return ((int) (BitsPerPixel / (BlockSize == 1 ? 8 : 0.5)), BlockSize);
        }

        public override string ToString()
        {
            return $"[DXGIPixelFormatInfo 0x{BitsPerPixel} 0x{BlockSize}]";
        }
    }
}
