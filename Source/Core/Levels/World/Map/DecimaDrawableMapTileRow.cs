using System.Runtime.Serialization;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.World.Map
{
    [DecimaFileType(0, "drawable-map-tile-row")]
    public class DecimaDrawableMapTileRow : IComplexStruct
    {
        [DataMember] public int EntryCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(EntryCount))]
        public DecimaDrawableMapTileSet[] Entries { get; set; }

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
        }
    }
}
