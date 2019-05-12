using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using ZeroDawn.Core;
using ZeroDawn.Helper;
using ZeroDawn.Helper.DirectX;

namespace ZeroDawn.Abstract.BaseImpl
{
    [DebuggerDisplay("Texture = {" + nameof(Name) + "} Size = {" + nameof(Width) + "}x{" + nameof(Height) + "}")]
    public class TextureBase : ITexture
    {
        public TextureBase(Span<byte> data, Span<byte> streamedData, DecimaPixelFormat pixelFormat, int width, int height, int mips, int streamedMips)
        {
            Format = pixelFormat.ToDXGI();
            Width = width;
            Height = height;
            Mips = mips;

            var dataMips = MathHelper.Swizzle(data, streamedMips, mips, Format, Width, Height);
            if (streamedData.Length > 0)
            {
                var dataMain = MathHelper.Swizzle(streamedData, 0, streamedMips, Format, Width, Height);
                var dataCombined = new Span<byte>(new byte[dataMips.Length + dataMain.Length]);
                dataMain.CopyTo(dataCombined);
                dataMips.CopyTo(dataCombined.Slice(dataMain.Length));
                Data = dataCombined.ToArray();
            }
            else
            {
                Data = dataMips.ToArray();
            }
        }

        public DXGIPixelFormat Format { get; }
        public int Width { get; }
        public int Height { get; }
        public int Mips { get; }
        public string Name { get; set; }
        public byte[] Data { get; }

        public Span<byte> ToDirectDraw()
        {
            var dds = new DDSImageHeader
            {
                Magic = 0x20534444,
                Size = 124,
                Flags = 0x1 | 0x2 | 0x4 | 0x1000 | 0x20000,
                Height = (uint) Height,
                Width = (uint) Width,
                LinearSize = 0,
                Depth = 0,
                MipmapCount = 1,
                Format = new DDSPixelFormat
                {
                    Size = 32,
                    Flags = 4,
                    FourCC = 0x30315844,
                    BitCount = 32,
                    RedMask = 0x0000FF00,
                    GreenMask = 0x00FF0000,
                    BlueMask = 0xFF000000,
                    AlphaMask = 0x000000FF
                },
                Caps1 = 0x1000,
                Caps2 = 0,
                Caps3 = 0,
                Caps4 = 0,
                Reserved2 = 0,
                DXGIFormat = (uint) Format,
                Dimension = DDSResourceDimension.TEXTURE2D,
                Misc = 0,
                MapSize = 1,
                Misc2 = 0
            };

            if (Mips > 1)
            {
                dds.MipmapCount = (uint) Mips;
                dds.Caps1 = 0x8 | 0x1000 | 0x400000;
            }

            return MemoryMarshal.AsBytes(new Span<DDSImageHeader>(new[] {dds}));
        }

        public void Save(Stream target)
        {
            target.Write(ToDirectDraw());
            target.Write(Data, 0, Data.Length);
        }

        public void Save(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using (var f = File.OpenWrite(path))
            {
                f.SetLength(0);
                Save(f);
            }
        }
    }
}
