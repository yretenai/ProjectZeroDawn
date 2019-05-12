using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ZeroDawn.Abstract;
using ZeroDawn.Core;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Helper
{
    public static class DecimaFileTypeFactory
    {
        /// <summary>
        ///     Loaded types
        /// </summary>
        public static IReadOnlyDictionary<DecimaFileTypeMagic, Type> TypeMap { get; private set; }

        /// <summary>
        ///     Loaded types by type
        /// </summary>
        public static IReadOnlyDictionary<Type, DecimaFileTypeMagic> TypeMapReverse { get; private set; }

        /// <summary>
        ///     Open Structured File by Core File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="managers"></param>
        /// <returns>
        ///     <see cref="IDecimaStructuredFile" />
        /// </returns>
        public static IDecimaStructuredFile OpenFile(DecimaCoreFile file, DecimaManagerCollection managers)
        {
            return OpenFile(file, file.FileType, managers);
        }

        /// <summary>
        ///     Open Structured File <typeparamref name="T" /> by Core File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="managers"></param>
        /// <typeparam name="T">
        ///     <see cref="IDecimaStructuredFile" />
        /// </typeparam>
        /// <returns>
        ///     <see cref="IDecimaStructuredFile" />
        /// </returns>
        public static T OpenFile<T>(DecimaCoreFile file, DecimaManagerCollection managers) where T : IDecimaStructuredFile
        {
            return OpenFile<T>(file, file.FileType, managers);
        }

        /// <summary>
        ///     Open Structured File by <paramref name="fileType" />
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileType"></param>
        /// <param name="managers"></param>
        /// <returns>
        ///     <see cref="IDecimaStructuredFile" />
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static IDecimaStructuredFile OpenFile(DecimaCoreFile file, DecimaFileTypeMagic fileType, DecimaManagerCollection managers)
        {
            if (TypeMap == default) LoadTypes();

            if (TypeMap == default) throw new InvalidOperationException("Failed to load types");

            if (!TypeMap.TryGetValue(fileType, out var type)) return default;

            var inst = (IDecimaStructuredFile) Activator.CreateInstance(type);
            inst.Work(file, managers);
            if (inst is IComplexStruct) ComplexReaderImpl.Read(inst, type, file, managers);
            Debug.Assert(file.Position == file.Length, "file.Position == file.Length");
            return inst;
        }

        /// <summary>
        ///     Open Structured File <typeparamref name="T" /> by <paramref name="fileType" />
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileType"></param>
        /// <param name="managers"></param>
        /// <typeparam name="T">
        ///     <see cref="IDecimaStructuredFile" />
        /// </typeparam>
        /// <returns>
        ///     <see cref="IDecimaStructuredFile" />
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static T OpenFile<T>(DecimaCoreFile file, DecimaFileTypeMagic fileType, DecimaManagerCollection managers) where T : IDecimaStructuredFile
        {
            if (TypeMapReverse == default) LoadTypes();

            if (TypeMapReverse == default) throw new InvalidOperationException("Failed to load types");

            if (!TypeMapReverse.TryGetValue(typeof(T), out var magic) || magic != fileType) return default;

            var inst = Activator.CreateInstance<T>();
            inst.Work(file, managers);
            if (inst is IComplexStruct) ComplexReaderImpl.Read(inst, typeof(T), file, managers);
            Debug.Assert(file.Position == file.Length, "file.Position == file.Length");
            return inst;
        }

        /// <summary>
        ///     Load types by <paramref name="loadSource" />
        /// </summary>
        /// <param name="loadSource"></param>
        public static void LoadTypes(Assembly loadSource = default)
        {
            var asm = loadSource ?? Assembly.GetAssembly(typeof(IDecimaStructuredFile));
            TypeMap = new ReadOnlyDictionary<DecimaFileTypeMagic, Type>(asm.GetTypes().Where(x => x.IsClass && !x.IsInterface && typeof(IDecimaStructuredFile).IsAssignableFrom(x)).Select(type => (type.GetCustomAttribute<DecimaFileTypeAttribute>()?.Magic, Type: type)).Where(x => x.Magic.HasValue).ToDictionary(x => x.Magic.Value, x => x.Type));
            TypeMapReverse = new ReadOnlyDictionary<Type, DecimaFileTypeMagic>(TypeMap.ToDictionary(x => x.Value, x => x.Key));
        }
    }
}
