using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.World
{
    [DecimaFileType(DecimaFileTypeMagic.LevelProperty, "level-property")]
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class DecimaLevelProperty : IDecimaStructuredFile, IComplexStruct
    {
        [DataMember] public DecimaString Name { get; set; }

        [DataMember] public int TagCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(TagCount))]
        public DecimaString[] Tags { get; set; }

        [DataMember] public int ReferenceCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(ReferenceCount))]
        public DecimaCoreFileRef<IDecimaStructuredFile>[] References { get; set; }

        [DataMember] public int WorkReferenceCount { get; set; }

        [DataMember]
        [ComplexInfo(nameof(WorkReferenceCount))]
        public DecimaCoreFileRef<IDecimaStructuredFile>[] WorkReferences { get; set; }

        [DataMember] public DecimaCoreFileRef<DecimaCollection> Collection { get; set; }

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
        }

        public void Dispose()
        {
            Core?.Dispose();
        }

        public DecimaCoreFile Core { get; set; }

        public virtual void Work(DecimaCoreFile data, DecimaManagerCollection managers)
        {
            if (data.FileType != DecimaFileTypeMagic.LevelProperty)
                throw new InvalidDataException("This is not a level property file");
            Core = data;
        }
    }
}
