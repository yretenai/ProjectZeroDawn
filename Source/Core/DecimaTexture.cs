using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core
{
    /// <summary>
    ///     Decima Texture stream
    /// </summary>
    /// <inheritdoc cref="ITextureProvider" />
    [DebuggerDisplay("Count = {" + nameof(TextureCount) + "} Name = {" + nameof(Name) + "}")]
    [DecimaFileType(DecimaFileTypeMagic.Texture, "texture")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DecimaTexture : ITextureProvider, IDecimaStructuredFile, IComplexStruct
    {
        [DataMember] public DecimaString Name { get; private set; }

        [DataMember] public DecimaTextureHeadless TextureData { get; protected set; }

        /// <inheritdoc />
        public override ITexture[] Textures { get; protected set; }

        /// <inheritdoc />
        public override int TextureCount => 1;

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
            TextureData.Data.Name = Name;
            Textures = new[] {TextureData.Data};
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
            if (data.FileType != DecimaFileTypeMagic.Texture)
                throw new InvalidDataException("This is not a texture file");
            Core = data;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Core?.Dispose();
        }
    }
}
