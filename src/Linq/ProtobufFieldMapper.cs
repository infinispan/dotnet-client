using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Google.Protobuf;

namespace Infinispan.Hotrod.Linq
{
    internal static class ProtobufFieldMapper
    {
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, string>> Cache = new();
        private static readonly ConcurrentDictionary<Type, string> TypeNameCache = new();

        public static string GetProtoFieldName(Type entityType, string csharpPropertyName)
        {
            var map = Cache.GetOrAdd(entityType, BuildMap);
            if (!map.TryGetValue(csharpPropertyName, out var protoName))
                throw new NotSupportedException(
                    $"Property '{csharpPropertyName}' does not map to a protobuf field on type {entityType.Name}.");
            return protoName;
        }

        public static string GetProtoTypeName(Type entityType)
        {
            return TypeNameCache.GetOrAdd(entityType, t =>
            {
                var instance = (IMessage)Activator.CreateInstance(t);
                return instance.Descriptor.FullName;
            });
        }

        private static IReadOnlyDictionary<string, string> BuildMap(Type type)
        {
            var instance = (IMessage)Activator.CreateInstance(type);
            var descriptor = instance.Descriptor;
            var dict = new Dictionary<string, string>();
            foreach (var field in descriptor.Fields.InFieldNumberOrder())
            {
                dict[field.PropertyName] = field.Name;
            }
            return dict;
        }
    }
}
