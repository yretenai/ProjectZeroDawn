using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using ZeroDawn.Core;
using ZeroDawn.Managers;

namespace ZeroDawn.Helper.ComplexReader
{
    public static class ComplexReaderImpl
    {
        public static Dictionary<Type, Dictionary<string, PropertyInfo>> Cache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public static Dictionary<Type, Dictionary<string, PropertyInfo>> CacheAll = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public static Dictionary<MemberInfo, ComplexInfoAttribute> InfoCache = new Dictionary<MemberInfo, ComplexInfoAttribute>();

        public static HashSet<Type> DecimaTypes = new HashSet<Type>();

        public static void Compile(Assembly loadSource = default)
        {
            var asm = loadSource ?? Assembly.GetAssembly(typeof(IComplexStruct));
            DecimaTypes = asm.GetTypes().Where(x => x.IsClass && typeof(IComplexStruct).IsAssignableFrom(x)).ToHashSet();
            foreach (var type in DecimaTypes)
            {
                GetTypeInfoAll(type);
                var properties = GetTypeInfo(type);
                foreach (var (_, property) in properties) GetInfo(property);
            }
        }

        public static object Read(object instance, Type type, DecimaCoreFile data, DecimaManagerCollection managers)
        {
            var allProperties = GetTypeInfoAll(type);
            var properties = GetTypeInfo(type);
            foreach (var (name, property) in properties)
            {
                var propertyType = property.PropertyType;
                var info = GetInfo(property);
                if (info?.Conditional != default && allProperties.TryGetValue(info.Conditional, out var conditionalProperty))
                    if (Convert.ToUInt64(conditionalProperty.GetValue(instance)) == 0)
                        continue;
                if (propertyType.IsArray || propertyType.FullName == "System.String")
                {
                    if (info == null)
                        throw new InvalidOperationException($"{type.FullName}.{name} is trying to load an array without {nameof(ComplexInfoAttribute)}");
                    var count = info.Count;
                    if (count == 0)
                    {
                        if (!allProperties.TryGetValue(info.FieldName, out var sizeProperty))
                            throw new InvalidOperationException($"{type.FullName}.{name} is trying to load an array with dynamic size property ");

                        count = Convert.ToInt64(sizeProperty.GetValue(instance));
                    }

                    if (propertyType.FullName == "System.String")
                        property.SetValue(instance, Encoding.UTF8.GetString(ReadData(info.FromUnknownData ? data.Unknown1 : data, typeof(byte), managers, count).Cast<byte>().ToArray()));
                    else
                        property.SetValue(instance, ReadData(info.FromUnknownData ? data.Unknown1 : data, propertyType.GetElementType(), managers, count));
                }
                else
                {
                    property.SetValue(instance, ReadData(info?.FromUnknownData == true ? data.Unknown1 : data, propertyType, managers, 1).GetValue(0));
                }
            }

            if (instance is IComplexStruct @struct) @struct.Read(data, managers);

            return instance;
        }

        public static ComplexInfoAttribute GetInfo(MemberInfo property)
        {
            if (!InfoCache.TryGetValue(property, out var info))
            {
                info = property.GetCustomAttribute<ComplexInfoAttribute>();
                InfoCache[property] = info;
            }

            return info;
        }

        public static Dictionary<string, PropertyInfo> GetTypeInfo(Type type)
        {
            if (!Cache.TryGetValue(type, out var info))
            {
                info = GetTypeInfoAll(type).Where(x => x.Value.GetCustomAttribute<DataMemberAttribute>() != null).ToDictionary(x => x.Key, x => x.Value);
                Cache[type] = info;
            }

            return info;
        }

        public static Dictionary<string, PropertyInfo> GetTypeInfoAll(Type type)
        {
            if (!CacheAll.TryGetValue(type, out var info))
            {
                var selectType = type;
                IEnumerable<PropertyInfo> t = Array.Empty<PropertyInfo>();
                while (typeof(IComplexStruct).IsAssignableFrom(selectType))
                {
                    t = selectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.DeclaredOnly).Concat(t);
                    selectType = selectType.BaseType;
                }

                info = t.ToDictionary(x => x.Name, x => x);
                CacheAll[type] = info;
            }

            return info;
        }

        public static Array ReadData(DecimaCoreFile data, Type propertyType, DecimaManagerCollection managers, long count)
        {
            if (propertyType.IsPrimitive || propertyType.IsEnum || propertyType.IsValueType)
                return StreamHelper.Read(propertyType, data, count);


            if (propertyType.FullName == "System.Guid")
            {
                var guidData = StreamHelper.ReadData(data, count * 0x10);
                var guidArray = new Guid[count];
                for (var i = 0; i < count; ++i) guidArray[i] = new Guid(guidData.Slice(i * 0x10, 0x10));
                return guidArray;
            }

            Debug.Assert(typeof(IComplexStruct).IsAssignableFrom(propertyType), "typeof(IComplexStruct).IsAssignableFrom(propertyType)");

            var instArray = Array.CreateInstance(propertyType, count);
            for (var i = 0; i < count; ++i)
            {
                var inst = Activator.CreateInstance(propertyType);
                instArray.SetValue(Read(inst, propertyType, data, managers), i);
            }

            return instArray;
        }
    }
}
