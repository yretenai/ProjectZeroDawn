using System;
using System.IO;
using ZeroDawn.Helper.DirectX;

namespace ZeroDawn.Abstract
{
    public interface ITexture
    {
        DXGIPixelFormat Format { get; }
        int Width { get; }
        int Height { get; }
        int Mips { get; }
        string Name { get; set; }
        byte[] Data { get; }
        Span<byte> ToDirectDraw();
        void Save(Stream target);
        void Save(string path);
    }
}
