using System;
using System.Text;

namespace Infinispan.Hotrod
{
    /// <summary>
    /// Fluent builder for creating a <see cref="Cache{K,V}"/> with string keys.
    /// Marshallers are inferred from the value type and encoding, or can be set explicitly.
    /// </summary>
    public class CacheBuilder<V>
    {
        private readonly InfinispanClient _client;
        private readonly string _name;
        private MediaType _encoding;
        private Marshaller<string> _keyMarshaller;
        private Marshaller<V> _valueMarshaller;

        internal CacheBuilder(InfinispanClient client, string name)
        {
            _client = client;
            _name = name;
        }

        public CacheBuilder<V> WithEncoding(MediaType encoding)
        {
            _encoding = encoding;
            return this;
        }

        public CacheBuilder<V> WithKeyMarshaller(Marshaller<string> marshaller)
        {
            _keyMarshaller = marshaller;
            return this;
        }

        public CacheBuilder<V> WithValueMarshaller(Marshaller<V> marshaller)
        {
            _valueMarshaller = marshaller;
            return this;
        }

        public Cache<string, V> Build()
        {
            if (_encoding == null)
                throw new InvalidOperationException("Encoding must be set via WithEncoding().");

            var keyM = _keyMarshaller ?? InferKeyMarshaller();
            var valM = _valueMarshaller ?? InferValueMarshaller();
            var cache = new Cache<string, V>(_client, keyM, valM, _name);
            cache.KeyMediaType = _encoding;
            cache.ValueMediaType = _encoding;
            return cache;
        }

        private Marshaller<string> InferKeyMarshaller()
        {
            if (_encoding == MediaType.Protostream || _encoding == MediaType.Protobuf)
                return new ProtostreamStringMarshaller();
            return new StringMarshaller(Encoding.UTF8);
        }

        private Marshaller<V> InferValueMarshaller()
        {
            var vType = typeof(V);

            if (typeof(Google.Protobuf.IMessage).IsAssignableFrom(vType))
            {
                var marshallerType = typeof(ProtobufMarshaller<>).MakeGenericType(vType);
                return (Marshaller<V>)Activator.CreateInstance(marshallerType);
            }

            if (vType == typeof(string))
                return (Marshaller<V>)(object)new StringMarshaller(Encoding.UTF8);

            if (vType == typeof(byte[]))
                return (Marshaller<V>)(object)ByteArrayMarshaller.Instance;

            throw new InvalidOperationException(
                $"Cannot infer value marshaller for type {vType.Name}. " +
                "Use WithValueMarshaller() to provide one explicitly.");
        }
    }
}
