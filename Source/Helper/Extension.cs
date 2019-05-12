using System;
using System.Collections.Generic;
using System.Linq;
using ZeroDawn.Abstract;
using ZeroDawn.Core;
using ZeroDawn.Helper.DirectX;
using ZeroDawn.Managers;

namespace ZeroDawn.Helper
{
    public static class Extension
    {
        public static ulong[] DecimaFileTypeMagicValues = Enum.GetValues(typeof(DecimaFileTypeMagic)).Cast<ulong>().ToArray();

        public static DXGIPixelFormat ToDXGI(this DecimaPixelFormat fmt)
        {
            switch (fmt)
            {
                case DecimaPixelFormat.BC1:
                    return DXGIPixelFormat.BC1_UNORM;
                case DecimaPixelFormat.BC2:
                    return DXGIPixelFormat.BC2_UNORM;
                case DecimaPixelFormat.BC3:
                    return DXGIPixelFormat.BC3_UNORM;
                case DecimaPixelFormat.BC4:
                    return DXGIPixelFormat.BC4_UNORM;
                case DecimaPixelFormat.BC5:
                    return DXGIPixelFormat.BC5_UNORM;
                case DecimaPixelFormat.BC7:
                    return DXGIPixelFormat.BC7_UNORM;
                case DecimaPixelFormat.R8G8B8A8:
                    return DXGIPixelFormat.R8G8B8A8_UNORM;
                case DecimaPixelFormat.A8:
                    return DXGIPixelFormat.A8_UNORM;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fmt), fmt, default);
            }
        }

        public static DecimaCoreFile OfType(this IEnumerable<DecimaCoreFile> array, DecimaFileTypeMagic fileType)
        {
            return array.FirstOrDefault(x => x.FileType == fileType);
        }

        public static T ToStructured<T>(this IEnumerable<DecimaCoreFile> array, DecimaManagerCollection managers) where T : IDecimaStructuredFile
        {
            if (DecimaFileTypeFactory.TypeMapReverse == default) DecimaFileTypeFactory.LoadTypes();

            if (DecimaFileTypeFactory.TypeMapReverse == default) throw new InvalidOperationException("Failed to load types");

            if (!DecimaFileTypeFactory.TypeMapReverse.TryGetValue(typeof(T), out var fileType)) return default;

            return array.OfType(fileType).ToStructured<T>(managers);
        }

        public static IEnumerable<Tuple<T, int>> WithIndex<T>(this IEnumerable<T> array)
        {
            return array.Select((x, y) => new Tuple<T, int>(x, y));
        }

        public static IEnumerable<DecimaCoreFile> OfTypes(this IEnumerable<DecimaCoreFile> array, DecimaFileTypeMagic fileType)
        {
            return array.Where(x => x.FileType == fileType);
        }

        public static IEnumerable<T> ToManyStructured<T>(this IEnumerable<DecimaCoreFile> array, DecimaManagerCollection managers) where T : IDecimaStructuredFile
        {
            if (DecimaFileTypeFactory.TypeMapReverse == default) DecimaFileTypeFactory.LoadTypes();

            if (DecimaFileTypeFactory.TypeMapReverse == default) throw new InvalidOperationException("Failed to load types");

            if (!DecimaFileTypeFactory.TypeMapReverse.TryGetValue(typeof(T), out var fileType)) return default;

            return array.OfTypes(fileType).Select(x => x.ToStructured<T>(managers));
        }

        public static bool IsValue(this DecimaFileTypeMagic magic)
        {
            return DecimaFileTypeMagicValues.Contains((ulong) magic);
        }
    }
}
