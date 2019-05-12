using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.World
{
    [DecimaFileType(DecimaFileTypeMagic.LevelTilesProperty, "level-tiles-property")]
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class DecimaLevelTilesProperty : DecimaLevelProperty
    {
        [DataMember] public int TileSize { get; set; }

        [DataMember] public int Unknown1 { get; set; }

        [DataMember] public int Unknown2 { get; set; }

        [DataMember] public int Unknown3 { get; set; }

        [DataMember] public int TileCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(TileCount))]
        public DecimaCoreFileRef<IDecimaStructuredFile>[] Tiles { get; set; }

        [DataMember] public int Unknown4 { get; set; }

        [DataMember] public int Unknown5 { get; set; }

        public override void Work(DecimaCoreFile data, DecimaManagerCollection managers)
        {
            if (data.FileType != DecimaFileTypeMagic.LevelTilesProperty)
                throw new InvalidDataException("This is not a level property file");
            Core = data;
        }
    }
}
