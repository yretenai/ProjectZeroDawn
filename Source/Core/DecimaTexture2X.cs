using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core
{
    /// <summary>
    ///     Decima Texture @2 stream
    /// </summary>
    /// <inheritdoc cref="ITextureProvider" />
    [DebuggerDisplay("Count = {" + nameof(TextureCount) + "} Name1 = {" + nameof(Name1) + "} Name2 = {" + nameof(Name2) + "}")]
    [DecimaFileType(DecimaFileTypeMagic.Texture2x, "texture-2x")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DecimaTexture2X : ITextureProvider, IDecimaStructuredFile, IComplexStruct
    {
        [DataMember] public DecimaString Name1 { get; private set; }

        [DataMember] public DecimaString Name2 { get; private set; }

        [DataMember] public int Width { get; private set; }

        [DataMember] public int Height { get; private set; }

        [DataMember] public int ImageSize1 { get; private set; }

        [DataMember] public int ImageSize2 { get; private set; }

        [DataMember]
        [ComplexInfo(nameof(TextureCount), Comment = "texture_count = 2 if image_size2 > 0 else 1")]
        public DecimaTextureHeadless[] TextureData { get; protected set; }

        /// <inheritdoc />
        public override ITexture[] Textures { get; protected set; }

        /// <inheritdoc />
        public override int TextureCount => ImageSize2 > 0 ? 2 : 1;

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
            TextureData.ElementAt(0).Data.Name = Name1;
            if (TextureData.Length == 2)
                TextureData.ElementAt(1).Data.Name = Name2;
            Textures = TextureData.Select(x => x.Data).ToArray();
        }

        /// <inheritdoc />
        public DecimaCoreFile Core { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="managers"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void Work(DecimaCoreFile data, DecimaManagerCollection managers)
        {
            if (data.FileType != DecimaFileTypeMagic.Texture2x)
                throw new InvalidDataException("This is not a texture x2 file");
            Core = data;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Core?.Dispose();
        }
    }
}
