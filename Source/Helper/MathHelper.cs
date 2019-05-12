using System;
using System.Collections.Generic;
using System.Linq;
using ZeroDawn.Helper.DirectX;

namespace ZeroDawn.Helper
{
    public static class MathHelper
    {
        private static readonly Dictionary<DXGIPixelFormat, (int bpp, int blockSize)> UnpackedDXGI = new Dictionary<DXGIPixelFormat, (int, int)>();

        public static bool IsPow2(int value)
        {
            var log = Math.Log(value, 2);
            var pow = Math.Pow(2, Math.Round(log));
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return pow == value;
        }

        private static int Morton(int t, int sx, int sy)
        {
            var aX = 1;
            var aY = 1;
            var aT = t;
            var oX = sx;
            var oY = sy;
            var oT = 0;
            var I = 0;
            while (oX > 1 || oY > 1)
            {
                if (oX > 1)
                {
                    oT += aY * (aT & 1);
                    aT >>= 1;
                    aY *= 2;
                    oX >>= 1;
                }

                if (oY > 1)
                {
                    I += aX * (aT & 1);
                    aT >>= 1;
                    aX *= 2;
                    oY >>= 1;
                }
            }

            return I * sx + oT;
        }

        public static Span<byte> Swizzle(Span<byte> data, int mipOffset, int mips, DXGIPixelFormat format, int imageX, int imageY)
        {
            if (!GetDXGIInfo(format, out var unpacked)) return default;

            var mipData = new Span<byte>(new byte[data.Length]);
            var dataOffset = 0;
            for (var mip = mipOffset; mip < mips; mip += 1)
            {
                var mipImageX = CalculateMip(mip, imageX);
                var mipImageY = CalculateMip(mip, imageY);
                var mipImageSize = CalculateSize(mipImageX, mipImageY, unpacked.bpp, unpacked.blockSize);
                var swizzledMipData = Swizzle(data.Slice(dataOffset, mipImageSize), unpacked.bpp, unpacked.blockSize, mipImageX, mipImageY);
                swizzledMipData.CopyTo(mipData.Slice(dataOffset, mipImageSize));
                dataOffset += mipImageSize;
            }

            return mipData.Slice(0, dataOffset);
        }

        private static int CalculateSize(int x, int y, int bpp, int blockSize)
        {
            var sy = y / blockSize;
            var ay = (sy + 7) / 8;
            var sx = x / blockSize;
            var ax = (sx + 7) / 8;
            return ay * ax * 64 * bpp;
        }

        private static int CalculateMip(int mip, int value)
        {
            for (var i = 0; i < mip; ++i) value /= 2;
            return value;
        }

        public static Span<byte> Swizzle(Span<byte> data, DXGIPixelFormat format, int imageX, int imageY)
        {
            return GetDXGIInfo(format, out var unpacked) ? Swizzle(data, unpacked.bpp, unpacked.blockSize, imageX, imageY) : default;
        }

        private static bool GetDXGIInfo(DXGIPixelFormat format, out (int bpp, int blockSize) unpacked)
        {
            if (UnpackedDXGI.TryGetValue(format, out unpacked)) return true;

            var attribute = typeof(DXGIPixelFormat).GetField(format.ToString("G")).GetCustomAttributes(false).OfType<DXGIPixelFormatInfoAttribute>().FirstOrDefault();
            if (attribute == null) return false;

            unpacked = attribute.Unpack();
            UnpackedDXGI[format] = unpacked;
            return true;
        }

        public static Span<byte> Swizzle(Span<byte> data, int bpp, int blockSize, int imageX, int imageY)
        {
            var deswizzled = new Span<byte>(new byte[data.Length]);
            var sy = imageY / blockSize;
            var sx = imageX / blockSize;
            var o = 0;
            for (var y = 0; y < (sy + 7) / 8; ++y)
            for (var x = 0; x < (sx + 7) / 8; ++x)
            for (var t = 0; t < 64; ++t)
            {
                var index = Morton(t, 8, 8);
                var mY = index / 8;
                var mX = index % 8;
                if (x * 8 + mX < sx && y * 8 + mY < sy)
                {
                    var destinationIndex = bpp * ((y * 8 + mY) * sx + x * 8 + mX);
                    if (destinationIndex != data.Length)
                    {
                        if (o > data.Length) o -= bpp;
                        data.Slice(o, bpp).CopyTo(deswizzled.Slice(destinationIndex, bpp));
                    }
                }

                o += bpp;
            }

            return deswizzled;
        }
    }
}
