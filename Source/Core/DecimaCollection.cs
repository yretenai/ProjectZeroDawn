using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core
{
    /// <summary>
    ///     Decima simple text table, basically a list of string hashes.
    /// </summary>
    [DecimaFileType(DecimaFileTypeMagic.Collection, "collection")]
    public class DecimaCollection : IDecimaStructuredFile, IComplexStruct
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public int EntryCount { get; private set; }

        /// <summary>
        /// </summary>
        [DataMember]
        [ComplexInfo(nameof(EntryCount))]
        public DecimaCoreFileRef<IDecimaStructuredFile>[] EntryRefsOriginal { get; private set; }

        public DecimaCoreFileRef<IDecimaStructuredFile>[] EntryRefs { get; private set; }

        public IDecimaStructuredFile[] Entries { get; private set; }

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
            var entriesActual = new List<DecimaCoreFileRef<IDecimaStructuredFile>>();
            foreach (var entry in EntryRefsOriginal)
                if (entry.LoadMethod == DecimaCoreLoadMethod.ImmediateCoreFile && entry.Ref?.FileType == DecimaFileTypeMagic.Collection)
                    entriesActual.AddRange(entry.Ref.ToStructured<DecimaCollection>(manager).EntryRefs);
                else
                    entriesActual.Add(entry);
            EntryRefs = entriesActual.ToArray();

            Entries = EntryRefs.Select(x => x.GetStruct(manager)).ToArray();
        }

        /// <inheritdoc />
        public DecimaCoreFile Core { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="managers"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void Work(DecimaCoreFile data, DecimaManagerCollection managers)
        {
            if (data.FileType != DecimaFileTypeMagic.Collection)
                throw new InvalidDataException("This is not a collection file");
            Core = data;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Core?.Dispose();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is DecimaCollection table) return EntryRefs.Equals(table.EntryRefs);

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return EntryRefs.GetHashCode();
        }
    }
}
