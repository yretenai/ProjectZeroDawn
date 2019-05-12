using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.World
{
    [DecimaFileType(DecimaFileTypeMagic.LevelFactProperty, "level-fact-property")]
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class DecimaLevelFactProperty : DecimaLevelProperty
    {
        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> Triggers { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> Resource { get; set; }

        [DataMember] public byte Unknown { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> Facts1 { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> Facts2 { get; set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> Facts3 { get; set; }

        public override void Work(DecimaCoreFile data, DecimaManagerCollection managers)
        {
            if (data.FileType != DecimaFileTypeMagic.LevelFactProperty)
                throw new InvalidDataException("This is not a level property file");
            Core = data;
        }
    }
}
