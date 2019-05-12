using System;
using System.Collections.Generic;
using System.Linq;
using ZeroDawn.Core;
using ZeroDawn.Core.Localized;
using ZeroDawn.Helper;

namespace ZeroDawn.Managers
{
    /// <summary>
    /// </summary>
    public class DecimaLocalizationManager : IDecimaManager
    {
        /// <summary>
        /// </summary>
        private readonly Dictionary<Guid, DecimaSimpleText> Cache = new Dictionary<Guid, DecimaSimpleText>();

        public void Dispose()
        {
            foreach (var (_, text) in Cache) text.Dispose();
            Cache.Clear();
        }

        /// <summary>
        ///     Get text by hash, otherwise load it into cache.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="file"></param>
        /// <param name="managers"></param>
        /// <returns></returns>
        public DecimaSimpleText GetText(Guid hash, string file, DecimaManagerCollection managers)
        {
            if (Cache.TryGetValue(hash, out var simple)) return simple;

            ImportText(file, managers);

            return Cache.TryGetValue(hash, out simple) ? simple : default;
        }

        /// <summary>
        ///     Import text strings from file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="managers"></param>
        public void ImportText(string name, DecimaManagerCollection managers)
        {
            if (!managers.GetManager<DecimaCacheManager>(out var caches)) return;
            using (var stream = caches.OpenFile(name.Substring(6)))
            {
                ImportStream(stream, managers);
            }
        }

        /// <summary>
        ///     Import text strings from by stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="managers"></param>
        public void ImportStream(DecimaCoreFile stream, DecimaManagerCollection managers)
        {
            if (stream == default) return;
            var texts = stream.Split().ToStructured<DecimaCollection>(managers);
            foreach (var text in texts.Entries.OfType<DecimaSimpleText>()) Cache[text.Core.Checksum] = text;
        }

        /// <summary>
        ///     Find text by hash
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool HasText(Guid hash)
        {
            return Cache.ContainsKey(hash);
        }
    }
}
