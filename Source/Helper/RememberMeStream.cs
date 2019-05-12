using System;
using System.IO;

namespace ZeroDawn.Helper
{
    /// <inheritdoc />
    /// <summary>
    ///     Remembers stream position
    /// </summary>
    public class RememberMeStream : IDisposable
    {
        private readonly long pos;
        private readonly Stream stream;

        /// <summary>
        /// </summary>
        /// <param name="target"></param>
        public RememberMeStream(Stream target)
        {
            stream = target;
            pos = stream.Position;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Dispose()
        {
            stream.Position = pos;
        }
    }
}
