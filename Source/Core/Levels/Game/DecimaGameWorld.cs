using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Core.Levels.World;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.Game
{
    /// <summary>
    ///     Decima game world info
    /// </summary>
    [DecimaFileType(DecimaFileTypeMagic.GameWorld, "game-world")]
    public class DecimaGameWorld : IComplexStruct, IDecimaStructuredFile
    {
        [DataMember] public DecimaString Name { get; private set; }

        [DataMember] public int Unknown1 { get; private set; } // 30

        [DataMember] public float Unknown2 { get; private set; } // 14000.0f

        [DataMember] public int Unknown3 { get; private set; } // 40000

        [DataMember] public int Unknown4 { get; private set; } // 5000

        [DataMember] [ComplexInfo(18)] public float[] Matrix { get; private set; }

        [DataMember] public DecimaCoreFileRef<DecimaLevelData> LevelData { get; private set; }

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
            // unused
        }

        public void Dispose()
        {
            Core?.Dispose();
        }

        public DecimaCoreFile Core { get; set; }

        public void Work(DecimaCoreFile data, DecimaManagerCollection managers)
        {
            if (data.FileType != DecimaFileTypeMagic.GameWorld)
                throw new InvalidDataException("This is not a game world file");
            Core = data;
        }
    }
}
