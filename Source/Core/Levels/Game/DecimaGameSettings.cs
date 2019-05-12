using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Levels.Game
{
    /// <summary>
    ///     Decima game settings list
    /// </summary>
    [DecimaFileType(DecimaFileTypeMagic.GameSettings, "game-settings")]
    public class DecimaGameSettings : IComplexStruct, IDecimaStructuredFile
    {
        [DataMember] public int PropertyCount { get; private set; }

        [DataMember]
        [ComplexInfo(nameof(PropertyCount))]
        public DecimaCoreFileRef<DecimaGameProperty>[] Properties { get; private set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> DefaultSystemAssets { get; private set; }

        [DataMember] public DecimaCoreFileRef<IDecimaStructuredFile> DefaultApplicationAssets { get; private set; }

        [DataMember] public int GameSettingCount { get; private set; }

        [DataMember]
        [ComplexInfo(nameof(GameSettingCount))]
        public DecimaCoreFileRef<IDecimaStructuredFile>[] GameSettings { get; private set; }

        [DataMember] public int TelemetrySettingCount { get; private set; }

        [DataMember]
        [ComplexInfo(nameof(TelemetrySettingCount))]
        public DecimaCoreFileRef<IDecimaStructuredFile>[] TelemetrySettings { get; private set; }

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
            if (data.FileType != DecimaFileTypeMagic.GameSettings)
                throw new InvalidDataException("This is not a game settings file");
            Core = data;
        }
    }
}
