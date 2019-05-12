using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Cintio;
using ZeroDawn.Core;
using ZeroDawn.Core.Levels.Game;
using ZeroDawn.Core.Levels.World;
using ZeroDawn.Core.Levels.World.Map;
using ZeroDawn.Helper;
using ZeroDawn.Helper.ComplexReader;
using ZeroDawn.Managers;

namespace ZeroDawn.Tool.LibraryTest
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal static class Program
    {
        private static readonly Regex PartRegex = new Regex(@"[\""].+?[\""]|[^ ]+", RegexOptions.Compiled);

        private static readonly Regex CamelCaseMap = new Regex("(?=\\p{Lu}\\p{Ll})|(?<=\\p{Ll})(?=\\p{Lu})", RegexOptions.Compiled);

        private static IEnumerable<string> CmdToArgs(string cmd)
        {
            return PartRegex.Matches(cmd).Select(x => x.Value).ToArray();
        }

        private static void Main()
        {
            var manager = new DecimaManagerCollection();
            InteractivePrompt.Run((cmd, list, completions) =>
            {
                try
                {
                    var args = CmdToArgs(cmd);
                    var argsEnumerable = args as string[] ?? args.ToArray();
                    var command = argsEnumerable.FirstOrDefault()?.ToLower();
                    switch (command)
                    {
                        case "exit":
                        case "quit":
                            manager.Clean();
                            Environment.Exit(0);
                            break;
                        case "unload":
                            manager.Clean();
                            return "Unloaded." + Environment.NewLine;
                        case "load":
                        {
                            var path = argsEnumerable.ElementAtOrDefault(1) ?? Environment.GetEnvironmentVariable("HZD_PACKED_PINK");
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} C:\Path" + Environment.NewLine;

                            var cache = manager.GetOrCreateManager<DecimaCacheManager>();
                            if (cache.Prefetch.IsValid)
                                return "Already loaded" + Environment.NewLine;
                            cache.LoadCaches(path);
                            cache.GenerateFileMap();
                            cache.LoadPrefetch(manager);
                            return "Loaded." + Environment.NewLine;
                        }
                        case "search-term":
                        {
                            var term = argsEnumerable.ElementAtOrDefault(1);
                            return string.Join(Environment.NewLine, manager.GetOrCreateManager<DecimaCacheManager>().Prefetch.Names.Where(x => x.Text.Contains(term))) + Environment.NewLine;
                        }
                        case "test-03":
                        {
                            var path = argsEnumerable.ElementAtOrDefault(1);
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} C:\Path" + Environment.NewLine;

                            if (!manager.GetOrCreateManager<DecimaCacheManager>().Prefetch.IsValid)
                                return @"Run load C:\Path first" + Environment.NewLine;
                            SaveMapTiles(manager, path);
                            break;
                        }
                        case "test-04":
                        {
                            var path = argsEnumerable.ElementAtOrDefault(1);
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} C:\Path" + Environment.NewLine;

                            if (!manager.GetOrCreateManager<DecimaCacheManager>().Prefetch.IsValid)
                                return @"Run load C:\Path first" + Environment.NewLine;
                            SaveUserInterfaceTex(manager, path);
                            break;
                        }
                        case "split-core":
                        {
                            var path = argsEnumerable.ElementAtOrDefault(1);
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} C:\Path.core C:\OutPath" + Environment.NewLine;
                            var outPath = argsEnumerable.ElementAtOrDefault(2);
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} {$"\"{path}\""} C:\OutPath" + Environment.NewLine;
                            DumpCore(path, outPath);
                            break;
                        }
                        case "open-core":
                        {
                            var path = argsEnumerable.ElementAtOrDefault(1);
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} C:\Path.core" + Environment.NewLine;
                            using (var stream = File.OpenRead(path))
                            using (var core = new DecimaCoreFile(stream))
                            {
                                foreach (var c in core.Split()) c.ToStructured(manager);
                            }

                            break;
                        }
                        case "save-type":
                        {
                            void SaveTypeM(CoreTuple tuple, ISet<long> hashSet, DecimaCacheManager decimaCacheManager, string s)
                            {
                                var (masterCore, hash, text) = tuple;
                                if (masterCore == default) return;
                                hashSet.Add(hash);
                                hashSet.Add(decimaCacheManager.GetFileHash(text + ".stream"));
                                var cores = masterCore.Split();

                                foreach (var (core, i) in cores.WithIndex())
                                {
                                    var dest = Path.Combine(s, core.FileTypeProxy, text);
                                    if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);

                                    var target = Path.Combine(dest, $"{i:X}_{core.Checksum:N}.core");

                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.OrangeRed, true, Console.Out, default, $"{target}... ");
                                    using (var stream = File.OpenWrite(target))
                                    {
                                        stream.SetLength(0);
                                        core.Dump(stream);
                                    }
                                }
                            }

                            var done = new HashSet<long>();
                            var cache = manager.GetOrCreateManager<DecimaCacheManager>();
                            if (!cache.Prefetch.IsValid)
                                return @"Run load C:\Path first" + Environment.NewLine;
                            var path = argsEnumerable.ElementAtOrDefault(1);
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} C:\Path.core" + Environment.NewLine;
                            foreach (var prefetchPath in cache.Prefetch)
                            {
                                Logger.Log24Bit(ConsoleSwatch.XTermColor.White, true, Console.Out, default, $"extracting {prefetchPath}... ");
                                try
                                {
                                    using (var tuple = cache.OpenFile(prefetchPath))
                                    {
                                        SaveTypeM(tuple, done, cache, path);
                                        Logger.Log24Bit(ConsoleSwatch.XTermColor.Magenta, true, Console.Out, default, "PASS");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.Red, true, Console.Out, default, $"ERROR: {ex.Message}");
                                }
                            }

                            path = Path.Combine(path, "unlisted");
                            foreach (var key in cache.FileMap.Keys)
                            {
                                if (!done.Add(key)) continue;
                                Logger.Log24Bit(ConsoleSwatch.XTermColor.White, true, Console.Out, default, $"extracting unlisted {key:X16}... ");
                                try
                                {
                                    using (var tuple = cache.OpenFile(key))
                                    {
                                        SaveTypeM(new CoreTuple(tuple, key), done, cache, path);
                                        Logger.Log24Bit(ConsoleSwatch.XTermColor.Magenta, true, Console.Out, default, "PASS");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.Red, true, Console.Out, default, $"ERROR: {ex.Message}");
                                }
                            }

                            break;
                        }
                        case "save-logical":
                        {
                            void Save(string filepath, DecimaCacheManager decimaCacheManager, string s)
                            {
                                using (var core = decimaCacheManager.OpenFile(filepath))
                                {
                                    decimaCacheManager.SaveFile(s, filepath, core);
                                }
                            }

                            var cache = manager.GetOrCreateManager<DecimaCacheManager>();
                            if (!cache.Prefetch.IsValid)
                                return @"Run load C:\Path first" + Environment.NewLine;
                            var path = argsEnumerable.ElementAtOrDefault(1);
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} C:\Path.core" + Environment.NewLine;
                            foreach (var prefetchPath in cache.Prefetch)
                            {
                                Logger.Log24Bit(ConsoleSwatch.XTermColor.White, false, Console.Out, default, $"extracting {prefetchPath}... ");
                                try
                                {
                                    Save(prefetchPath, cache, path);
                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.Magenta, true, Console.Out, default, "PASS");
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.Red, true, Console.Out, default, $"ERROR: {ex.Message}");
                                }
                            }

                            break;
                        }
                        case "test-01":
                        {
                            var cache = manager.GetOrCreateManager<DecimaCacheManager>();
                            if (!cache.Prefetch.IsValid)
                                return @"Run load C:\Path first" + Environment.NewLine;
                            using (var tuple = cache.OpenFile("localized/sentences/commgiraffe/commgiraffe_activity/simpletext.core"))
                            {
                                var (core, _) = tuple;
                                // ReSharper disable once UnusedVariable
                                var collection = core.Split().ToStructured<DecimaCollection>(manager);
                            }

                            break;
                        }
                        case "test-02":
                        {
                            var cache = manager.GetOrCreateManager<DecimaCacheManager>();
                            if (!cache.Prefetch.IsValid)
                                return @"Run load C:\Path first" + Environment.NewLine;
                            using (var tuple = cache.OpenFile("localized/categories/ingame/ingame_shared.core"))
                            {
                                var (core, _) = tuple;
                                // ReSharper disable once UnusedVariable
                                var collection = core.Split().ToStructured<DecimaCollection>(manager);
                            }

                            break;
                        }
                        case "dump-struct":
                        {
                            var path = argsEnumerable.ElementAtOrDefault(1);
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} C:\Path.core" + Environment.NewLine;
                            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                            DumpStruct(path);
                            break;
                        }
                        case "test-05":
                        {
                            var cache = manager.GetOrCreateManager<DecimaCacheManager>();
                            if (!cache.Prefetch.IsValid)
                                return @"Run load C:\Path first" + Environment.NewLine;
                            using (var tuple = cache.OpenFile("levels/game.core"))
                            {
                                var (core, _) = tuple;
                                var world = core.Split().ToStructured<DecimaGameWorld>(manager);

                                DecimaLevelData level = world.LevelData;

                                var properties = level.Properties.Select(x => (DecimaLevelProperty) x).Concat(level.VanillaData.Select(x => (DecimaLevelProperty) x)).Concat(level.DLC1Data.Select(x => (DecimaLevelProperty) x)).ToArray();

                                var unused = properties.First().Collection.GetStruct(manager);
                            }

                            break;
                        }
                        case "save-map-vanilla":
                        case "save-map-dlc1":
                        {
                            var cache = manager.GetOrCreateManager<DecimaCacheManager>();
                            if (!cache.Prefetch.IsValid)
                                return @"Run load C:\Path first" + Environment.NewLine;
                            var path = argsEnumerable.ElementAtOrDefault(1);
                            if (string.IsNullOrWhiteSpace(path))
                                return $@"Usage: {command} C:\Path" + Environment.NewLine;
                            using (var tuple = cache.OpenFile("levels/game.core"))
                            {
                                var (core, _) = tuple;
                                var world = core.Split().ToStructured<DecimaGameWorld>(manager);

                                DecimaLevelData level = world.LevelData;

                                var alwaysLoaded = (DecimaLevelProperty) (command == "save-map-vanilla" ? level.VanillaData : level.DLC1Data).FirstOrDefault(x => ((DecimaLevelProperty) x).Name.Text.ToLower().EndsWith("always loaded"));
                                var collection = (DecimaCollection) alwaysLoaded.Collection;
                                var tiles = collection.Entries.OfType<DecimaDrawableMapTiles>().First();
                                var target = Path.Combine(path, command == "save-map-vanilla" ? "vanilla" : "dlc1", "map-tiles", "color");
                                var targetHeight = Path.Combine(path, command == "save-map-vanilla" ? "vanilla" : "dlc1", "map-tiles", "height");
                                foreach (var (row, x) in tiles.Rows.WithIndex())
                                foreach (var (set, y) in row.Entries.WithIndex())
                                {
                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.White, false, Console.Out, default, $"{x}, {y}: ");
                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.Purple, false, Console.Out, default, "COLOR ");
                                    ((DecimaTexture) set.Color).TextureData.Data.Save(Path.Combine(target, $"{x},{y}.dds"));
                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.Green, false, Console.Out, default, "OK ");
                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.Purple, false, Console.Out, default, "HEIGHT ");
                                    ((DecimaTexture) set.Height).TextureData.Data.Save(Path.Combine(targetHeight, $"{x},{y}.dds"));
                                    Logger.Log24Bit(ConsoleSwatch.XTermColor.Green, true, Console.Out, default, "OK ");
                                }
                            }
                        }
                            break;
                        default:
                            return $"Unknown command {command}" + Environment.NewLine;
                    }

                    return Environment.NewLine;
                }
                catch (Exception e)
                {
                    Logger.Log24Bit(ConsoleSwatch.XTermColor.Red, false, Console.Out, default, $"ERROR: {e.Message}");
                    if (Debugger.IsAttached) throw;
                    return Environment.NewLine;
                }
            }, "> ", "ProjectZeroDawn Interop", new List<string>
            {
                "dump-struct",
                "exit",
                "load",
                "unload",
                "split-core",
                "search-term",
                "split-core",
                "open-core",
                "save-type",
                "test-01",
                "test-02",
                "test-03",
                "test-04",
                "test-05"
            });
        }

        private static void DumpStruct(string path)
        {
            ComplexReaderImpl.Compile();
            var done = new HashSet<Type>();
            var typeMap = ComplexReaderImpl.DecimaTypes.ToDictionary(x => x, x => x.GetCustomAttribute<DecimaFileTypeAttribute>()?.Name);
            foreach (var type in ComplexReaderImpl.DecimaTypes)
                DumpStructImpl(path, type, done, typeMap);
            DumpStructEnum(path, typeof(DecimaFileTypeMagic), true);
        }

        private static void DumpStructImpl(string path, Type type, ISet<Type> done, IReadOnlyDictionary<Type, string> typeMap)
        {
            if (type == typeof(Guid)) type = typeof(GuidShim);
            if (!done.Add(type))
                return;
            if (type.IsEnum)
            {
                DumpStructEnum(path, type);
            }
            else if (type.IsValueType)
            {
                DumpStructLiteral(path, type, done, typeMap);
            }
            else if (!type.IsInterface)
            {
                var typeInfo = type.GetCustomAttribute<DecimaFileTypeAttribute>();
                if (typeInfo == null) return;
                var lines = new List<string> {string.Empty};
                if (typeInfo.Magic != 0)
                {
                    var typeMagic = (ulong) typeInfo.Magic;
                    lines.Add($"File Type = 0x{typeMagic:X16}");
                    lines.Add(string.Empty);
                }

                var thisTypeName = GetTypeName(typeMap, type, default);
                lines.Add($"Decima::{thisTypeName} {{");
                foreach (var (name, property) in ComplexReaderImpl.GetTypeInfo(type))
                {
                    var typeName = GetTypeName(typeMap, property.PropertyType, property);
                    var line = $"{typeName} {Formalize(name)};";
                    var info = ComplexReaderImpl.GetInfo(property);
                    if (info != null)
                    {
                        if (!string.IsNullOrWhiteSpace(info.Conditional))
                            line = $"[optional] {line} // only {info.Comment ?? $"when {Formalize(info.Conditional)} is not 0"}";
                        else if (!string.IsNullOrWhiteSpace(info.Comment))
                            line = $"{line} // {info.Comment}";
                    }

                    if (!property.PropertyType.IsPrimitive) DumpStructImpl(path, property.PropertyType, done, typeMap);
                    lines.Add("  " + line);
                }

                lines.Add("}");
                lines.Add(string.Empty);
                File.WriteAllText(Path.Combine(path, typeInfo.Name + ".txt"), string.Join("\n", lines));
            }
        }

        private static void DumpStructEnum(string path, Type type, bool order = false)
        {
            var baseType = type.GetEnumUnderlyingType();
            var lines = new List<string> {string.Empty};
            var thisTypeName = GetTypeName(new Dictionary<Type, string>(), type, default);
            var baseTypeName = GetTypeName(new Dictionary<Type, string>(), baseType, default);
            lines.Add($"Decima::{thisTypeName} : {baseTypeName} {{");
            var keys = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            IEnumerable<KeyValuePair<string, ulong>> dict = Enumerable.Range(0, keys.Length).ToDictionary(x => keys[x], x => Convert.ToUInt64(values.GetValue(x)));
            if (order)
                // ReSharper disable once UseDeconstructionOnParameter
                dict = dict.OrderBy(x => x.Key);
            foreach (var (name, value) in dict) lines.Add($"  {name} = 0x{value:X},");
            lines.Add("}");
            File.WriteAllText(Path.Combine(path, thisTypeName.Replace("_", "-") + ".txt"), string.Join("\n", lines));
        }

        private static void DumpStructLiteral(string path, Type type, ISet<Type> done, IReadOnlyDictionary<Type, string> typeMap)
        {
            var lines = new List<string> {string.Empty};
            var thisTypeName = GetTypeName(typeMap, type, default);
            lines.Add($"Decima::{thisTypeName} {{");
            foreach (var member in type.GetFields())
            {
                var typeName = GetTypeName(typeMap, member.FieldType, member);
                var line = $"{typeName} {Formalize(member.Name)};";
                if (!member.FieldType.IsPrimitive) DumpStructImpl(path, member.FieldType, done, typeMap);
                lines.Add("  " + line);
            }

            lines.Add("}");
            File.WriteAllText(Path.Combine(path, thisTypeName.Replace("_", "-") + ".txt"), string.Join("\n", lines));
        }

        private static string Formalize(string t)
        {
            var text = CamelCaseMap.Replace(t, "_").ToLower();
            while (text.StartsWith("_")) text = text.Substring(1);
            if (text.StartsWith("u_")) text = "u" + text.Substring(2);
            return text;
        }

        private static string GetTypeName(IReadOnlyDictionary<Type, string> typeMap, Type t, MemberInfo member)
        {
            if (t.IsInterface) return "T";
            if (t.IsArray)
            {
                var text = GetTypeName(typeMap, t.GetElementType(), default) + "[";
                if (member != default)
                {
                    var info = ComplexReaderImpl.GetInfo(member);
                    text += string.IsNullOrWhiteSpace(info.FieldName) ? info.Count.ToString("X") : Formalize(info.FieldName);
                }

                return text + "]";
            }

            if (t.IsGenericType && t.IsConstructedGenericType)
            {
                var text = GetTypeName(typeMap, t.GetGenericTypeDefinition(), default) + "<";
                text += string.Join(", ", t.GetGenericArguments().Select(x => GetTypeName(typeMap, x, default)).ToArray());
                return text + ">";
            }

            if (!typeMap.TryGetValue(t, out var typeName) || typeName == null)
                typeName = Formalize(t.Name);
            try
            {
                if (typeName.StartsWith("decima_")) typeName = typeName.Substring(7);
            }
            catch
            {
                // 
            }

            return typeName.Replace("-", "_");
        }

        private static void DumpCore(string path, string outPath)
        {
            using (var stream = File.OpenRead(path))
            {
                var cores = new DecimaCoreFile(stream).Split().ToArray();
                if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath);
                foreach (var (core, i) in cores.WithIndex())
                {
                    var dest = Path.Combine(outPath, $"{i:X2}_{core.FileTypeProxy}_{core.Checksum:N}.core");
                    using (var dump = File.OpenWrite(dest))
                    {
                        core.Dump(dump);
                    }

                    Logger.Log24Bit(ConsoleSwatch.XTermColor.MediumPurple, true, Console.Out, default, $"{i:X2}_{core.FileTypeProxy}.core");
                }
            }
        }

        private static void SaveMapTiles(DecimaManagerCollection manager, string path)
        {
            if (!manager.GetManager<DecimaCacheManager>(out var caches)) return;
            foreach (var file in caches.Prefetch.Where(x => x.Contains("ingamemap")))
                using (var data = caches.OpenFile(file))
                {
                    try
                    {
                        Logger.Log24Bit(ConsoleSwatch.XTermColor.White, false, Console.Out, default, $"extracting {file}... ");

                        var tex = data.Core?.ToStructured<DecimaTexture>(manager);
                        if (tex == default)
                        {
                            Logger.Log24Bit(ConsoleSwatch.XTermColor.Magenta, true, Console.Out, default, "PASS");
                            continue;
                        }

                        var texture = tex.ElementAt(0);
                        texture.Save(Path.Combine(path, "map_textures", Path.GetDirectoryName(file), $"{texture.Name}.dds"));
                        Logger.Log24Bit(ConsoleSwatch.XTermColor.Purple, false, Console.Out, default, $"{texture.Name} ");
                        Logger.Log24Bit(ConsoleSwatch.XTermColor.Green, true, Console.Out, default, "OK");
                    }
                    catch (TargetInvocationException ex)
                    {
                        Logger.Log24Bit(ConsoleSwatch.XTermColor.Red, true, Console.Out, default, $"ERROR: {ex.InnerException.Message}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log24Bit(ConsoleSwatch.XTermColor.Red, true, Console.Out, default, $"ERROR: {ex.Message}");
                    }
                }
        }

        private static void SaveUserInterfaceTex(DecimaManagerCollection manager, string path)
        {
            if (!manager.GetManager<DecimaCacheManager>(out var caches)) return;
            foreach (var file in caches.Prefetch.Where(x => x.Contains("interface/textures")))
                using (var data = caches.OpenFile(file))
                {
                    try
                    {
                        Logger.Log24Bit(ConsoleSwatch.XTermColor.White, false, Console.Out, default, $"extracting {file}... ");

                        var tex = data.Core?.ToStructured<DecimaTexture2X>(manager);
                        if (tex == default)
                        {
                            Logger.Log24Bit(ConsoleSwatch.XTermColor.Magenta, true, Console.Out, default, "PASS");
                            continue;
                        }

                        foreach (var (texture, i) in tex.WithIndex())
                        {
                            texture.Save(Path.Combine(path, "textures", file, $"{texture.Name}_{i}.dds"));
                            Logger.Log24Bit(ConsoleSwatch.XTermColor.Purple, false, Console.Out, default, $"{texture.Name} ");
                        }

                        Logger.Log24Bit(ConsoleSwatch.XTermColor.Green, true, Console.Out, default, "OK");
                    }
                    catch (TargetInvocationException ex)
                    {
                        Logger.Log24Bit(ConsoleSwatch.XTermColor.Red, true, Console.Out, default, $"ERROR: {ex.InnerException.Message}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log24Bit(ConsoleSwatch.XTermColor.Red, true, Console.Out, default, $"ERROR: {ex.Message}");
                    }
                }
        }

        private struct GuidShim
        {
#pragma warning disable 649
            public int A;
            public short B;
            public short C;
            public byte D;
            public byte E;
            public byte F;
            public byte G;
            public byte H;
            public byte I;
            public byte J;
            public byte K;
#pragma warning restore 649
        }
    }
}
