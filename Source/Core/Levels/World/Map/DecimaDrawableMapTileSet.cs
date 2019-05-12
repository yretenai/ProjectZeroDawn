using System.Runtime.Serialization;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.World.Map
{
    [DecimaFileType(0, "drawable-map-tile-set")]
    public class DecimaDrawableMapTileSet : IComplexStruct
    {
        [DataMember] public DecimaCoreFileRef<DecimaTexture> Color { get; set; }

        [DataMember] public DecimaCoreFileRef<DecimaTexture> Height { get; set; }

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
        }
    }
}
