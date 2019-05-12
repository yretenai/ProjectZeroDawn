using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.World.Map
{
    [DecimaFileType(DecimaFileTypeMagic.DrawableMapTiles, "drawable-map-tiles")]
    public class DecimaDrawableMapTiles : IDecimaStructuredFile, IComplexStruct
    {
        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> UIEffectResource { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> UIEffectResource2x { get; set; }

        [DataMember] public DecimaCoreFileRef<DecimaTexture> DefaultBackground { get; set; }

        [DataMember] public DecimaCoreFileRef<DecimaTexture> DefaultBackground2x { get; set; }

        [DataMember] public int Unknown1 { get; set; }

        [DataMember] public int PosY { get; set; }

        [DataMember] public int PosX { get; set; }

        [DataMember] public int Unknown2 { get; set; }

        [DataMember] public int Unknown3 { get; set; }

        [DataMember] public int NegY { get; set; }

        [DataMember] public int NegX { get; set; }

        [DataMember] public int Unknown4 { get; set; }

        [DataMember] public int RowCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(RowCount))]
        public DecimaDrawableMapTileRow[] Rows { get; set; }

        [DataMember] public DecimaCoreFileRef<DecimaTexture> Clouds { get; set; }

        [DataMember] public DecimaCoreFileRef<DecimaTexture> Vignette { get; set; }

        [DataMember] public DecimaCoreFileRef<DecimaTexture> MapBorder { get; set; }

        [DataMember] public DecimaCoreFileRef<DecimaTexture> WorldZoom { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> Unknown5 { get; set; }

        [DataMember] public int Unknown6 { get; set; }

        [DataMember] [ComplexInfo(0x1003)] public byte[] Unknown7 { get; set; } // ZERO?! Not really.

        [DataMember] public float Unknown8 { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> ProjectionSettings { get; set; }

        [DataMember] public int MapBorderResourceCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(MapBorderResourceCount))]
        public DecimaCoreFileRef<IDecimaStructuredFile>[] MapBorderResources { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> MapSettings { get; set; }

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
        }

        public void Dispose()
        {
            Core?.Dispose();
        }

        public DecimaCoreFile Core { get; set; }

        public void Work(DecimaCoreFile data, DecimaManagerCollection managers)
        {
            if (data.FileType != DecimaFileTypeMagic.DrawableMapTiles)
                throw new InvalidDataException("This is not a drawable map tiles file");
            Core = data;
        }
    }
}
