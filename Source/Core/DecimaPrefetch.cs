using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace ZeroDawn.Core
{
    /// <inheritdoc cref="IEnumerable{T}" />
    /// <summary>
    ///     Decima prefetch cache file
    /// </summary>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DecimaFileType(DecimaFileTypeMagic.Prefetch, "prefetch")]
    public class DecimaPrefetch : IEnumerable<string>, IDecimaStructuredFile, IComplexStruct
    {
        /// <summary>
        ///     Sanity check
        /// </summary>
        public bool IsValid { get; private set; }

        [DataMember] public int NameCount { get; private set; }

        /// <summary>
        ///     List of filenames
        /// </summary>
        [DataMember]
        [ComplexInfo(nameof(NameCount))]
        public DecimaString[] Names { get; private set; } = Array.Empty<DecimaString>();

        [DataMember] public int SizeCount { get; private set; }

        /// <summary>
        ///     List of file sizes
        /// </summary>
        [DataMember]
        [ComplexInfo(nameof(SizeCount))]
        public int[] Sizes { get; private set; } = Array.Empty<int>();

        [DataMember] public int IndexCount { get; private set; }

        /// <summary>
        ///     List of file indices
        /// </summary>
        [DataMember]
        [ComplexInfo(nameof(IndexCount))]
        public int[] Indices { get; private set; } = Array.Empty<int>();

        /// <summary>
        ///     File count
        /// </summary>
        public int Count => NameCount;

        /// <summary>
        ///     Access file by index
        /// </summary>
        /// <param name="i"></param>
        public string this[int i] => Names[i];

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
            IsValid = true;
        }

        /// <inheritdoc />
        public DecimaCoreFile Core { get; set; }

        /// <summary>
        ///     Load prefetch data by stream
        /// </summary>
        /// <param name="data"></param>
        /// <param name="managers"></param>
        /// <exception cref="InvalidDataException">when data magic does not match file type</exception>
        public void Work(DecimaCoreFile data, DecimaManagerCollection managers)
        {
            if (IsValid) return;
            if (data.FileType != DecimaFileTypeMagic.Prefetch)
                throw new InvalidDataException("This is not a prefetch file");
            Core = data;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Core?.Dispose();
        }

        /// <inheritdoc />
        public IEnumerator<string> GetEnumerator()
        {
            return Names.Select(x => (string) x).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
