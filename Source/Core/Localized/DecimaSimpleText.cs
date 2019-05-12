using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core.Localized
{
    /// <summary>
    ///     Decima localized simple text file.
    /// </summary>
    [DecimaFileType(DecimaFileTypeMagic.SimpleText, "simple-text")]
    [DebuggerDisplay("{" + nameof(English) + "}")]
    public class DecimaSimpleText : IDecimaStructuredFile, IComplexStruct
    {
        [DataMember] public DecimaSmallString English { get; private set; }

        [DataMember] public DecimaSmallString French { get; private set; }

        [DataMember] public DecimaSmallString Spanish { get; private set; }

        [DataMember] public DecimaSmallString German { get; private set; }

        [DataMember] public DecimaSmallString Italian { get; private set; }

        [DataMember] public DecimaSmallString Dutch { get; private set; }

        [DataMember] public DecimaSmallString Portuguese { get; private set; }

        [DataMember] public DecimaSmallString ChineseTraditional { get; private set; }

        [DataMember] public DecimaSmallString Korean { get; private set; }

        [DataMember] public DecimaSmallString Russian { get; private set; }

        [DataMember] public DecimaSmallString Polish { get; private set; }

        [DataMember] public DecimaSmallString Norwegian { get; private set; }

        [DataMember] public DecimaSmallString Finnish { get; private set; }

        [DataMember] public DecimaSmallString Swedish { get; private set; }

        [DataMember] public DecimaSmallString Danish { get; private set; }

        [DataMember] public DecimaSmallString Japanese { get; private set; }

        [DataMember] public DecimaSmallString SpanishMexico { get; private set; }

        [DataMember] public DecimaSmallString PortugueseBrazil { get; private set; }

        [DataMember] public DecimaSmallString Unknown { get; private set; }

        [DataMember] public DecimaSmallString Arabic { get; private set; }

        [DataMember] public DecimaSmallString ChineseSimplified { get; private set; }

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
            Debug.Assert(Unknown == "", "Unknown == ''");
        }

        /// <inheritdoc />
        public DecimaCoreFile Core { get; set; }

        /// <summary>
        ///     Load text file by stream
        /// </summary>
        /// <param name="data"></param>
        /// <param name="managers"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void Work(DecimaCoreFile data, DecimaManagerCollection managers)
        {
            if (data.FileType != DecimaFileTypeMagic.SimpleText)
                throw new InvalidDataException("This is not a simple text file");
            Core = data;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Core?.Dispose();
        }

        /// <summary>
        ///     To string by language enum.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public string ToString(DecimaLanguageEnum language)
        {
            switch (language)
            {
                case DecimaLanguageEnum.English:
                default:
                    return English;
                case DecimaLanguageEnum.French:
                    return French;
                case DecimaLanguageEnum.Spanish:
                    return Spanish;
                case DecimaLanguageEnum.German:
                    return German;
                case DecimaLanguageEnum.Italian:
                    return Italian;
                case DecimaLanguageEnum.Dutch:
                    return Dutch;
                case DecimaLanguageEnum.Portuguese:
                    return Portuguese;
                case DecimaLanguageEnum.ChineseTraditional:
                    return ChineseTraditional;
                case DecimaLanguageEnum.Korean:
                    return Korean;
                case DecimaLanguageEnum.Russian:
                    return Russian;
                case DecimaLanguageEnum.Polish:
                    return Polish;
                case DecimaLanguageEnum.Norwegian:
                    return Norwegian;
                case DecimaLanguageEnum.Finnish:
                    return Finnish;
                case DecimaLanguageEnum.Swedish:
                    return Swedish;
                case DecimaLanguageEnum.Danish:
                    return Danish;
                case DecimaLanguageEnum.Japanese:
                    return Japanese;
                case DecimaLanguageEnum.SpanishMexico:
                    return SpanishMexico;
                case DecimaLanguageEnum.PortugueseBrazil:
                    return PortugueseBrazil;
                case DecimaLanguageEnum.Unknown:
                    return Unknown;
                case DecimaLanguageEnum.Arabic:
                    return Arabic;
                case DecimaLanguageEnum.ChineseSimplified:
                    return ChineseSimplified;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return English;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is DecimaSimpleText text) return text.English == English;

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return English.GetHashCode();
        }
    }
}
