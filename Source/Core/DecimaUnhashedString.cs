using System.Diagnostics;
using System.Runtime.Serialization;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core
{
    /// <summary>
    ///     Decima string reader
    /// </summary>
    [DebuggerDisplay("{" + nameof(Text) + "}")]
    [DecimaFileType(0, "unhashed-string")]
    public class DecimaUnhashedString : IComplexStruct
    {
        /// <summary>
        ///     Length of the string
        /// </summary>
        [DataMember]
        public int Size { get; private set; }

        /// <summary>
        ///     String text
        /// </summary>
        [DataMember]
        [ComplexInfo(nameof(Size))]
        public string Text { get; private set; }

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case DecimaSmallString decimaSmallString:
                    return decimaSmallString.Text == Text;
                case DecimaUnhashedString decimaUnhashedString:
                    return decimaUnhashedString.Text == Text;
                case DecimaString decimaString:
                    return decimaString.Text == Text;
                case string @string:
                    return @string == Text;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Text;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        /// <summary>
        ///     Return source string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static implicit operator string(DecimaUnhashedString source)
        {
            return source.Text;
        }
    }
}
