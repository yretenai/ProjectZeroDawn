namespace ZeroDawn
{
    public static class DecimaConstants
    {
        public const string PREFETCH_CACHE = "prefetch/fullgame.prefetch.core";

        /// <summary>
        ///     Murmurhash3x64_128 salt.
        /// </summary>
        public const uint HASH_SALT = 0x2A;

        /// <summary>
        ///     BIN file
        /// </summary>
        public const uint DECIMA_CACHE_FILE_MAGIC = 0x20304050;
    }
}
