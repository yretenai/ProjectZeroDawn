using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ZeroDawn.Helper
{
    public static class StreamHelper
    {
        private static readonly ConcurrentDictionary<Type, int> SizeCache = new ConcurrentDictionary<Type, int>();

        private static readonly ConcurrentDictionary<Type, MethodInfo> ReadCache = new ConcurrentDictionary<Type, MethodInfo>();

        private static readonly ConcurrentDictionary<Type, Type> EnumCache = new ConcurrentDictionary<Type, Type>();

        private static readonly MethodInfo ReadMethod = typeof(StreamHelper).GetMethod("Read", 1, new[] {typeof(Stream), typeof(long)});

        /// <summary>
        ///     Read single struct from stream
        /// </summary>
        /// <param name="s"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Read<T>(Stream s) where T : struct
        {
            return Read<T>(s, 1)[0];
        }

        /// <summary>
        ///     Read <see cref="arraySize">N size</see> entries from stream
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arraySize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static unsafe T[] Read<T>(Stream s, long arraySize) where T : struct
        {
            if (arraySize == 0) return Array.Empty<T>();

            var size = GetSize<T>();

            var dataSize = size * arraySize;

            if (dataSize == 0) return new T[size];

            Debug.Assert(dataSize > 0, "dataSize > 0");

            var buf = dataSize > 0x100000 ? new Span<byte>(new byte[dataSize]) : stackalloc byte[(int) dataSize];
            s.Read(buf);
            return MemoryMarshal.Cast<byte, T>(buf).ToArray();
        }

        public static Array Read(Type t, Stream s, long arraySize)
        {
            if (t.IsEnum) t = GetEnumType(t);

            if (!ReadCache.TryGetValue(t, out var method))
            {
                method = ReadMethod.MakeGenericMethod(t);
                ReadCache[t] = method;
            }

            return method.Invoke(default, new object[] {s, arraySize}) as Array;
        }

        private static Type GetEnumType(Type type)
        {
            if (!type.IsEnum) return type;
            if (!EnumCache.TryGetValue(type, out var t))
            {
                t = type.GetEnumUnderlyingType();
                EnumCache[type] = t;
            }

            return t;
        }

        /// <summary>
        ///     Read <see cref="arraySize">N size</see> entries from stream
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arraySize"></param>
        /// <returns></returns>
        public static Span<byte> ReadData(Stream s, long arraySize)
        {
            if (arraySize == 0) return Array.Empty<byte>();

            var buf = new Span<byte>(new byte[arraySize]);
            s.Read(buf);
            return buf;
        }

        /// <summary>
        ///     Get cached unmanaged size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetSize<T>() where T : struct
        {
            if (SizeCache.TryGetValue(typeof(T), out var size)) return size;
            size = Marshal.SizeOf<T>();
            SizeCache.TryAdd(typeof(T), size);
            return size;
        }
    }
}
