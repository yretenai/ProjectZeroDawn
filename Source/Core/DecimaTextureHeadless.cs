using System;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Abstract.BaseImpl;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core
{
    [DecimaFileType(0, "texture-base")]
    public class DecimaTextureHeadless : IComplexStruct
    {
        [DataMember] public short Unknown1 { get; set; }

        [DataMember] public ushort Width { get; set; }

        [DataMember] public ushort Height { get; set; }

        [DataMember] public short Unknown2 { get; set; }

        [DataMember] public byte TotalMips { get; set; }

        [DataMember] public DecimaPixelFormat Format { get; set; }

        [DataMember] public short Unknown4 { get; set; }

        [DataMember] public byte Unknown5 { get; set; }

        [DataMember] public short Unknown6 { get; set; }

        [DataMember] public DecimaCoreLoadMethod Method { get; set; }

        [DataMember] public Guid Checksum { get; set; }

        [DataMember] public int BufferSize { get; set; }

        [DataMember] public int TotalDataSize { get; set; }

        [DataMember] public int EmbeddedImageSize { get; set; }

        [DataMember] public int StreamedImageSize { get; set; }

        [DataMember] public int StreamedMips { get; set; }

        [DataMember]
        [ComplexInfo(nameof(EmbeddedImageSize))]
        public byte[] ImageData { get; set; }

        [DataMember]
        [ComplexInfo(Conditional = nameof(StreamedMips))]
        public DecimaStream StreamData { get; set; }

        public ITexture Data { get; private set; }

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
            // ReSharper disable once MergeConditionalExpression
            var streamData = StreamData == default ? Span<byte>.Empty : StreamData.Data.Span.Slice(0, StreamedImageSize);
            Data = new TextureBase(ImageData, streamData, Format, Width & 0xFFF, Height & 0xFFF, TotalMips, StreamedMips);
        }
    }
}
