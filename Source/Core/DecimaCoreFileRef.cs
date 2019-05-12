using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using ZeroDawn.Abstract;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Core
{
    [DecimaFileType(0, "file-reference")]
    public class DecimaCoreFileRef<T> : IDisposable, IComplexStruct where T : class, IDecimaStructuredFile
    {
        [DataMember] public DecimaCoreLoadMethod LoadMethod { get; private set; }

        [DataMember]
        [ComplexInfo(Conditional = nameof(LoadMethod))]
        public Guid Checksum { get; private set; }

        [DataMember]
        [ComplexInfo(Conditional = nameof(IsLoadedFile), Comment = "when load_method >= 2")]
        public DecimaString RefFile { get; private set; }

        public bool IsLoadedFile => LoadMethod == DecimaCoreLoadMethod.ImmediateCoreFile || LoadMethod == DecimaCoreLoadMethod.CoreFile;

        private DecimaCoreFile RefData { get; set; }
        public DecimaManagerCollection Managers { private get; set; }
        public DecimaCoreFile Ref => RefData ?? GetRef(Managers);

        public void Read(DecimaCoreFile file, DecimaManagerCollection manager)
        {
            Managers = manager;
            switch (LoadMethod)
            {
                case DecimaCoreLoadMethod.WorkOnly:
                case DecimaCoreLoadMethod.NotPresent:
                    break;
                case DecimaCoreLoadMethod.Embedded:
                    RefData = file.Split().FirstOrDefault(x => x.Checksum == Checksum);
                    Debug.Assert(RefData != default, "Ref != default", nameof(DecimaCoreLoadMethod.Embedded));
                    break;
                case DecimaCoreLoadMethod.ImmediateCoreFile:
                case DecimaCoreLoadMethod.CoreFile:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            RefData?.Dispose();
            if (disposing) RefData = default;
        }

        ~DecimaCoreFileRef()
        {
            Dispose(false);
        }

        public T GetStruct(DecimaManagerCollection managers)
        {
            Managers = managers;
            return Ref?.ToStructured(managers) as T;
        }

        public T GetStruct()
        {
            return Ref?.ToStructured(Managers) as T;
        }

        public static implicit operator T(DecimaCoreFileRef<T> @this)
        {
            return @this.GetStruct(@this.Managers);
        }

        public DecimaCoreFile GetRef(DecimaManagerCollection manager)
        {
            Managers = manager;
            if (RefData == default)
            {
                if (string.IsNullOrWhiteSpace(RefFile)) return default;
                if (!manager.GetManager<DecimaCacheManager>(out var caches)) return default;
                var data = caches.OpenFile(RefFile);
                if (data != null)
                {
                    RefData = data.Core.Split().FirstOrDefault(x => x.Checksum == Checksum);
                    Debug.Assert(RefData != default, "Ref != default", nameof(DecimaCoreLoadMethod.CoreFile));
                }
            }

            return RefData;
        }

        public override string ToString()
        {
            return $"[DecimaCoreRef<{typeof(T)}>:{LoadMethod} {Checksum:D}{(RefFile == null ? "" : " " + RefFile)}{(Ref != null ? " " + Ref : "")}]";
        }
    }
}
