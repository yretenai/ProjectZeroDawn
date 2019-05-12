using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Managers;
using ZeroDawn.Struct;

namespace ZeroDawn.Core
{
    /// <inheritdoc />
    /// <summary>
    ///     Decima Core-wrapped file.
    /// </summary>
    public class DecimaCoreFile : Stream
    {
        private static readonly Dictionary<DecimaFileTypeMagic, int> DirtyCoreTypes = new Dictionary<DecimaFileTypeMagic, int>
        {
            {DecimaFileTypeMagic.EntityMissionManager, 0x2},
            {DecimaFileTypeMagic.EntitySinglePlayerDeathCamera, 0x2},
            {DecimaFileTypeMagic.EntityFlyOverCamera, 0x2},
            {DecimaFileTypeMagic.EntityFXTemplate, 0x2},
            {DecimaFileTypeMagic.CollisionTrigger, 0x4},
            {DecimaFileTypeMagic.FactCollisionTrigger, 0x4},
            {DecimaFileTypeMagic.PhysicsCollisionResource, 0x4},
            {DecimaFileTypeMagic.HitResponseArea, 0x2},
            {DecimaFileTypeMagic.EntityPlayerCharacter, 0x2}
        };

        /// <inheritdoc />
        /// <summary>
        ///     Load core file by stream
        /// </summary>
        /// <param name="buffer"></param>
        public DecimaCoreFile(Stream buffer)
        {
            BaseStream = buffer;
            Header = StreamHelper.Read<DecimaCoreHeader>(buffer);
            Checksum = Header.GetChecksum();

            if (DirtyCoreTypes.TryGetValue(Header.FileTypeMagic, out var dirtyBytes))
            {
                BaseStream.Position -= 0x10;
                var unkData = StreamHelper.Read<byte>(BaseStream, dirtyBytes);
                Header.SetChecksum(StreamHelper.Read<byte>(BaseStream, 0x10));
                Unknown1 = new DecimaCoreFile(unkData, this);
            }
        }

        private DecimaCoreFile(byte[] unknown1, DecimaCoreFile parent)
        {
            BaseStream = new MemoryStream(unknown1);
            Header = parent.Header;
            IsDirtyData = true;
            Checksum = Header.GetChecksum();
        }

        private DecimaCoreFile(Stream buffer, DecimaCoreHeader header, byte[] unknown1, DecimaCoreFile parent)
        {
            BaseStream = buffer;
            Header = header;
            Checksum = Header.GetChecksum();
            IsSplit = true;
            Parent = parent;
            Unknown1 = unknown1?.Length > 0 ? new DecimaCoreFile(unknown1, this) : null;
        }

        private bool IsDirtyData { get; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DecimaCoreFile Unknown1 { get; }

        /// <summary>
        ///     Underlying data header
        /// </summary>
        public DecimaCoreHeader Header { get; }

        /// <summary>
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DecimaCoreFile[] Siblings { get; private set; }

        /// <summary>
        /// </summary>
        public DecimaCoreFile Parent { get; }

        /// <summary>
        ///     Declared file type
        /// </summary>
        public DecimaFileTypeMagic FileType => Header.FileTypeMagic;

        /// <summary>
        ///     Declared file type (for debugger)
        /// </summary>
        public string FileTypeProxy => Header.FileTypeMagic.ToString(Header.FileTypeMagic.IsValue() ? "G" : "X");

        /// <summary>
        ///     Data stream
        /// </summary>
        public Stream BaseStream { get; }

        /// <summary>
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Guid Checksum { get; }

        /// <inheritdoc />
        public override bool CanRead => BaseStream.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => BaseStream.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override long Length => IsDirtyData ? BaseStream.Length : Header.DataSize - 0x10;

        /// <summary>
        ///     The actual length of the stream
        /// </summary>
        public long TrueLength => BaseStream.Length;

        /// <summary>
        ///     True if the file is split already
        /// </summary>
        public bool IsSplit { get; private set; }

        /// <inheritdoc />
        public override long Position
        {
            get => IsDirtyData ? BaseStream.Position : BaseStream.Position - StreamHelper.GetSize<DecimaCoreHeader>();
            set => BaseStream.Position = IsDirtyData ? value : value + StreamHelper.GetSize<DecimaCoreHeader>();
        }

        private IDecimaStructuredFile Structured { get; set; }

        public bool Disposed { get; private set; }

        ~DecimaCoreFile()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Close(disposing);
        }

        public void Close(bool disposing, bool firstChance = true)
        {
            if (Disposed) return;
            BaseStream.Dispose();
            Disposed = true;
            if (IsDirtyData) return;
            if (firstChance)
            {
                if (Parent == default)
                {
                    foreach (var sibling in Siblings ?? Array.Empty<DecimaCoreFile>())
                        sibling?.Close(disposing, false);
                    if (disposing) Siblings = new DecimaCoreFile[0];
                }
                else
                {
                    Parent.Close(disposing);
                }
            }

            Unknown1?.Dispose();

            IsSplit = false;
            if (disposing) Siblings = new DecimaCoreFile[0];
        }

        private void SetSiblings(DecimaCoreFile[] siblings)
        {
            Siblings = siblings;
        }

        /// <inheritdoc />
        public override void Flush()
        {
            BaseStream.Flush();
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (IsDirtyData) return BaseStream.Seek(offset, origin);
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return BaseStream.Seek(offset + StreamHelper.GetSize<DecimaCoreHeader>(), origin);
                case SeekOrigin.Current:
                case SeekOrigin.End:
                    return BaseStream.Seek(offset, origin);
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), origin, default);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Does not support writing
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void SetLength(long value)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Does not support writing
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        /// <summary>
        ///     Dump entire base stream to target
        /// </summary>
        /// <param name="target"></param>
        public void Dump(Stream target)
        {
            using (new RememberMeStream(BaseStream))
            {
                BaseStream.Position = 0;
                BaseStream.CopyTo(target);
            }
        }

        /// <summary>
        ///     Parse as structured file
        /// </summary>
        /// <returns>
        ///     <see cref="IDecimaStructuredFile" />
        /// </returns>
        public IDecimaStructuredFile ToStructured(DecimaManagerCollection managers)
        {
            if (IsDirtyData) return default;
            if (!IsSplit)
                return Split().ElementAtOrDefault(0)?.ToStructured(managers);
            return Structured ?? (Structured = DecimaFileTypeFactory.OpenFile(this, managers));
        }

        /// <summary>
        ///     Parse as structured file <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T">
        ///     <see cref="IDecimaStructuredFile" />
        /// </typeparam>
        /// <returns>
        ///     <typeparamref name="T" />
        /// </returns>
        public T ToStructured<T>(DecimaManagerCollection managers) where T : IDecimaStructuredFile
        {
            if (IsDirtyData) return default;
            if (!IsSplit)
            {
                var o = Split().ElementAtOrDefault(0);
                return o == default ? default : o.ToStructured<T>(managers);
            }

            return (T) (!(Structured is T) ? Structured = DecimaFileTypeFactory.OpenFile<T>(this, managers) : Structured);
        }

        /// <summary>
        ///     Split core file up into multiple parts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DecimaCoreFile> Split()
        {
            if (IsDirtyData) return Array.Empty<DecimaCoreFile>();
            try
            {
                var headerSize = StreamHelper.GetSize<DecimaCoreHeader>();
                // if it's already split or we're the only entry
                if (IsSplit || Siblings != default) return Siblings;
                if (TrueLength == Length + headerSize || Length > TrueLength || Length < 0)
                {
                    IsSplit = true;
                    Siblings = new[] {this};
                    return Siblings;
                }

                var list = new List<DecimaCoreFile>();
                using (new RememberMeStream(BaseStream))
                {
                    BaseStream.Position = 0;
                    while (BaseStream.Position != BaseStream.Length)
                    {
                        var pre = BaseStream.Position;
                        var header = StreamHelper.Read<DecimaCoreHeader>(BaseStream);
                        var unkData = new byte[] { };
                        if (DirtyCoreTypes.TryGetValue(header.FileTypeMagic, out var dirtyBytes))
                        {
                            BaseStream.Position -= 0x10;
                            unkData = StreamHelper.Read<byte>(BaseStream, dirtyBytes);
                            header.SetChecksum(StreamHelper.Read<byte>(BaseStream, 0x10));
                        }

                        BaseStream.Position = pre;
                        var buffer = new byte[header.DataSize + headerSize - 0x10];
                        BaseStream.Read(buffer, 0, buffer.Length);
                        list.Add(new DecimaCoreFile(new MemoryStream(buffer) {Position = headerSize + unkData.Length}, header, unkData, this));
                    }
                }

                list.ForEach(x => x.SetSiblings(list.ToArray()));

                Siblings = list.ToArray();
                return Siblings;
            }
            catch
            {
                return Array.Empty<DecimaCoreFile>();
            }
        }

        public override string ToString()
        {
            return $"[DecimaCore{(IsDirtyData ? "Dirty" : "")} {FileTypeProxy} {Checksum:D}{(Structured != null ? " " + Structured : "")}]";
        }
    }
}
