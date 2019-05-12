using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Murmur;
using ZeroDawn.Core;
using ZeroDawn.Helper;

namespace ZeroDawn.Managers
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class DecimaCacheManager : IDecimaManager
    {
        private readonly Murmur128 Hasher = MurmurHash.Create128( /* 42 */ DecimaConstants.HASH_SALT, true, AlgorithmPreference.X64);

        /// <summary>
        ///     List of cache bin files keyed to the hash of its filename.
        /// </summary>
        public Dictionary<long, DecimaCache> CacheFiles { get; private set; } = new Dictionary<long, DecimaCache>();

        /// <summary>
        ///     Prefetch list data
        /// </summary>
        public DecimaPrefetch Prefetch { get; private set; } = new DecimaPrefetch();

        /// <summary>
        ///     File Map to cache Bin file
        /// </summary>
        public Dictionary<long, long> FileMap { get; private set; } = new Dictionary<long, long>();

        /// <summary>
        ///     State
        /// </summary>
        public bool Disposed { get; private set; }

        /// <inheritdoc />
        /// <summary>
        ///     Clean up
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Load specific cache file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>success</returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public bool LoadCache(string filename)
        {
            try
            {
                var hash = GetFileHash(Path.GetFileNameWithoutExtension(filename));
                if (CacheFiles.ContainsKey(hash)) return true;

                var cache = new DecimaCache(filename);
                CacheFiles[hash] = cache;
                return true;
            }
            catch (InvalidDataException)
            {
                return false;
            }
        }

        /// <summary>
        ///     Load all valid cache bin files from directory
        /// </summary>
        /// <param name="directory"></param>
        public void LoadCaches(string directory)
        {
            while (true)
            {
                if (!Directory.Exists(directory)) return;

                if (Directory.Exists(Path.Combine(directory, "packed_pink")))
                {
                    directory = Path.Combine(directory, "packed_pink");
                    continue;
                }

                foreach (var file in Directory.GetFiles(directory, "*.bin", SearchOption.TopDirectoryOnly)) LoadCache(file);
                break;
            }
        }

        /// <summary>
        ///     Generate file lookup for finding files in multiple cache bins
        /// </summary>
        public void GenerateFileMap()
        {
            var comparer = new FileMapComparer();
            foreach (var (hash, cache) in CacheFiles.OrderBy(x => x.Value.Header.LoadOrder))
                FileMap = FileMap.Union(cache.RecordTable.ToDictionary(x => x.Hash, x => hash), comparer).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        ///     Load prefetch cache file
        /// </summary>
        public void LoadPrefetch(DecimaManagerCollection managers)
        {
            using (var prefetchData = OpenFile(DecimaConstants.PREFETCH_CACHE))
            {
                Prefetch = prefetchData.Core.ToStructured<DecimaPrefetch>(managers);
            }
        }

        /// <summary>
        ///     Save file to path/filename.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="file"></param>
        public void SaveFile(string path, string filename, DecimaCoreFile file)
        {
            var target = Path.Combine(path, filename);
            var targetDir = Path.GetDirectoryName(target);
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

            using (var fs = File.OpenWrite(target))
            {
                fs.SetLength(file.TrueLength);
                file.Dump(fs);
            }
        }

        /// <summary>
        ///     Open file by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>(file stream, filename hash)</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public CoreTuple OpenFile(string path)
        {
            try
            {
                var hash = GetFileHash(path);
                return (OpenFile(hash), hash, path);
            }
            catch (FileNotFoundException e)
            {
                if (!path.EndsWith(".core")) return OpenFile(path + ".core");

                throw new FileNotFoundException($"Cannot find file {path}", e);
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
            if (FileMap.TryGetValue(hash, out var cacheHash))
                return CacheFiles[cacheHash].OpenFile(hash);

            throw new FileNotFoundException($"Cannot find hash {hash:X16}");
        }

        /// <summary>
        ///     Get file hash from string
        /// </summary>
        /// <param name="value"></param>
        /// <returns>hash</returns>
        public long GetFileHash(string value)
        {
            if (!value.EndsWith("\0")) value = value + "\0";

            return BitConverter.ToInt64(Hasher.ComputeHash(Encoding.ASCII.GetBytes(value)), 0);
        }

        /// <summary>
        ///     GC safe dispose wrapper
        /// </summary>
        /// <param name="disposing">truth check</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;

            Hasher?.Dispose();

            Prefetch.Dispose();

            foreach (var (_, cache) in CacheFiles) cache?.Dispose();

            if (disposing)
            {
                CacheFiles.Clear();
                CacheFiles = default;
                FileMap.Clear();
                FileMap = default;
                Prefetch = default;
            }

            Disposed = true;
        }

        ~DecimaCacheManager()
        {
            Dispose(false);
        }
    }
}
