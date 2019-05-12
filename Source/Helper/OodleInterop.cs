using System;
using System.Runtime.InteropServices;

namespace ZeroDawn.Helper
{
    public static class OodleInterop
    {
        
#if DARWIN
        public const string OODLE_LIBRARY = "oo2core_5_?.dylib"; // ?
#elif LINUX
        public const string OODLE_LIBRARY = "oo2core_5_?.a"; // ?
#else //elif WIN64
        public const string OODLE_LIBRARY = "oo2core_5_win64.dll";
#endif

        [DllImport(OODLE_LIBRARY)]
        private static extern int OodleLZ_Decompress(IntPtr buffer, long bufferSize, IntPtr outputBuffer, long outputBufferSize, uint a, uint b, ulong c, uint d, uint e, uint f, uint g, uint h, uint i, uint threadModule);

        /// <summary>
        ///     Decompress oodle2.5 compressed data
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <param name="outputBuffer"></param>
        /// <param name="outputSize"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static int Decompress(IntPtr buffer, long size, IntPtr outputBuffer, long outputSize)
        {
            return OodleLZ_Decompress(buffer, size, outputBuffer, outputSize, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3);
        }
    }
}
