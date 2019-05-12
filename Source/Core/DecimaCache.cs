using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZeroDawn.Helper;
using ZeroDawn.Struct.Cache;

namespace ZeroDawn.Core
{
    /// <inheritdoc />
    /// <summary>
    ///     Packed and compressed Decima BIN file
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public unsafe class DecimaCache : IDisposable
    {
        private readonly object Lock = new object();

        /// <summary>
        ///     Open bin by path
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="InvalidDataException"></exception>
        public DecimaCache(string filename)
        {
            BaseStream = File.OpenRead(filename);
            Header = StreamHelper.Read<DecimaCacheHeader>(BaseStream);

            if (Header.MagicBytes != DecimaConstants.DECIMA_CACHE_FILE_MAGIC)
                throw new InvalidDataException($"{Path.GetFileNameWithoutExtension(filename)} is not a Decima BIN file");

            BaseStream.Position = 0x28;

            RecordTable = new List<DecimaCacheRecordTableEntry>(StreamHelper.Read<DecimaCacheRecordTableEntry>(BaseStream, Header.RecordTableSize)).AsReadOnly();
            RecordTableByHash = RecordTable.Select((x, y) => new KeyValuePair<long, long>(x.Hash, y)).ToDictionary(x => x.Key, x => x.Value);

            BaseStream.Position = 0x28 + Header.RecordTableSize * 0x20;

            BlockTable = new List<DecimaCacheBlockTableEntry>(StreamHelper.Read<DecimaCacheBlockTableEntry>(BaseStream, Header.BlockTableSize).OrderBy(x => x.ChunkOffset)).AsReadOnly();
        }

        /// <summary>
        ///     Underlying stream
        /// </summary>
        private Stream BaseStream { get; }

        /// <summary>
        ///     Data header
        /// </summary>
        public DecimaCacheHeader Header { get; }

        /// <summary>
        ///     List of file records
        /// </summary>
        public IReadOnlyList<DecimaCacheRecordTableEntry> RecordTable { get; private set; }

        /// <summary>
        ///     Record table lookup by hash
        /// </summary>
        public IReadOnlyDictionary<long, long> RecordTableByHash { get; private set; }

        /// <summary>
        ///     List of blocks
        /// </summary>
        public IReadOnlyList<DecimaCacheBlockTableEntry> BlockTable { get; private set; }

        /// <summary>
        ///     State
        /// </summary>
        public bool Disposed { get; private set; }

        /// <inheritdoc />
        /// <summary>
        ///     Cleanup stream
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     GC safe dispose wrapper
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;

            BaseStream?.Dispose();

            if (disposing)
            {
                RecordTable = default;
                RecordTableByHash = default;
                BlockTable = default;
            }

            Disposed = true;
        }

        ~DecimaCache()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Decompress entire BIN file
        /// </summary>
        /// <param name="target"></param>
        public void Decompress(Stream target)
        {
            target.SetLength(Header.UncompressedSize);
            foreach (var block in BlockTable)
            {
                var blockData = OpenBlock(block);
                target.Position = block.ChunkOffset;
                target.Write(blockData);
            }
        }

        /// <summary>
        ///     decompress block
        /// </summary>
        /// <param name="block"></param>
        /// <returns>byte buffer</returns>
        private Span<byte> OpenBlock(DecimaCacheBlockTableEntry block)
        {
            lock (Lock)
            {
                BaseStream.Position = block.Offset;
                var size = block.Size;
                var sizeC = block.ChunkSize;
                var data = new Span<byte>(new byte[size]);
                var outputData = new Span<byte>(new byte[sizeC]);
                BaseStream.Read(data);
                fixed (byte* pin = data)
                fixed (byte* outputPin = outputData)
                {
                    OodleInterop.Decompress((IntPtr) pin, size, (IntPtr) outputPin, sizeC);
                    return outputData;
                }
            }
        }

        /// <summary>
        ///     Open file by hash
        /// </summary>
        /// <param name="hash"></param>
        /// <returns>file stream</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public DecimaCoreFile OpenFile(long hash)
        {
            if (!RecordTableByHash.TryGetValue(hash, out var recordIndex))
                throw new FileNotFoundException($"Cannot find hash {hash:X16}");

            var record = RecordTable[(int) recordIndex];

            var (chunk, index) = BlockTable.Select((x, y) => new KeyValuePair<DecimaCacheBlockTableEntry, int>(x, y)).First(x =>
            {
                var (key, _) = x;
                return key.ChunkOffset + key.ChunkSize > record.Offset;
            });

            var buffer = new MemoryStream(record.Size);
            while (buffer.Position < record.Size)
            {
                var block = OpenBlock(chunk);
                var start = 0;
                if (chunk.ChunkOffset < record.Offset) start = (int) (record.Offset - chunk.ChunkOffset);

                var end = block.Length - start;
                if (end + buffer.Position > record.Size) end = (int) (record.Size - buffer.Position);

                buffer.Write(block.Slice(start, end));
                index += 1;

                if (index >= BlockTable.Count) break;

                chunk = BlockTable[index];
            }

            // Debug.Assert(buffer.Length == record.Size, "buffer.Length == record.Size");

            buffer.Position = 0;
            return new DecimaCoreFile(buffer);
        }
    }
}
