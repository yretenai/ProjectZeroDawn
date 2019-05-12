using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.Game
{
    /// <summary>
    ///     Decima game property link
    /// </summary>
    [DecimaFileType(DecimaFileTypeMagic.GameProperty, "game-property")]
    public class DecimaGameProperty : IComplexStruct, IDecimaStructuredFile
    {
        [DataMember] public DecimaString Name { get; private set; }

        [DataMember] public int LinkCount { get; private set; }

        [DataMember]
        [ComplexInfo(nameof(LinkCount))]
        public DecimaCoreFileRef<DecimaGameWorld>[] Links { get; private set; }

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
            if (data.FileType != DecimaFileTypeMagic.GameProperty)
                throw new InvalidDataException("This is not a game properties file");
            Core = data;
        }
    }
}
