using System;
using System.Runtime.Serialization;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core
{
    /// <summary>
    ///     Core-associated stream files
    /// </summary>
    [DecimaFileType(0, "stream-info")]
    public class DecimaStream : IComplexStruct
    {
        private Memory<byte> BackedData = Memory<byte>.Empty;

        /// <summary>
        ///     Data stream
        /// </summary>
        private DecimaManagerCollection Manager;

        public Memory<byte> Data => BackedData.IsEmpty ? BackedData = Load() : BackedData;

        /// <summary>
        ///     Filename of the stream
        /// </summary>
        [DataMember]
        public DecimaUnhashedString Name { get; private set; }

        [DataMember] public int Offset { get; private set; }

        [DataMember] public int Unknown1 { get; private set; }

        [DataMember] public int Size { get; private set; }

        [DataMember] public int Unknown2 { get; private set; }

        /// <summary>
        ///     Length of the data stream
        /// </summary>
        public int Length => Size;

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
            Manager = manager;
        }

        private Memory<byte> Load()
        {
            if (!Manager.GetManager<DecimaCacheManager>(out var caches)) return Memory<byte>.Empty;

            Memory<byte> backed;

            using (var data = caches.OpenFile(Name.Text.Substring(6)))
            {
                var (stream, _) = data;
                if (stream == default) return Memory<byte>.Empty;
                stream.BaseStream.Position = Offset;
                backed = new Memory<byte>(new byte[Size]);
                stream.BaseStream.Read(backed.Span);
            }

            return backed;
        }

        /// <summary>
        ///     Retrieve data as bytes
        /// </summary>
        /// <returns></returns>
        public byte[] AsBytes()
        {
            return Data.ToArray();
        }

        /// <summary>
        ///     Retrieve data as bytes from <paramref name="start" />
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public byte[] AsBytes(int start)
        {
            return Data.Span.Slice(start).ToArray();
        }

        /// <summary>
        ///     Retrieve data as <paramref name="length" /> bytes from <paramref name="start" />
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] AsBytes(int start, int length)
        {
            return Data.Span.Slice(start, length).ToArray();
        }
    }
}
