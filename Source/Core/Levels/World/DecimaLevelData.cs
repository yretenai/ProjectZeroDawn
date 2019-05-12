using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.World
{
    [DecimaFileType(DecimaFileTypeMagic.LevelData, "level-data")]
    public class DecimaLevelData : IComplexStruct, IDecimaStructuredFile
    {
        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> CopyAreaInfo { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> CopyArea { get; set; }

        [DataMember] public int PropertyCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(PropertyCount))]
        public DecimaCoreFileRef<DecimaLevelProperty>[] Properties { get; set; }

        [DataMember] public int VanillaDataCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(VanillaDataCount))]
        public DecimaCoreFileRef<DecimaLevelProperty>[] VanillaData { get; set; }


        [DataMember] public int DLC1DataCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(DLC1DataCount))]
        public DecimaCoreFileRef<DecimaLevelProperty>[] DLC1Data { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> Graph { get; set; }

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
            if (data.FileType != DecimaFileTypeMagic.LevelData)
                throw new InvalidDataException("This is not a game level file");
            Core = data;
        }
    }
}
