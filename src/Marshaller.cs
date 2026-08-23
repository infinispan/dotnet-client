using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Org.Infinispan.Protostream;

namespace Infinispan.Hotrod
{
    /// <summary>
    /// Marshaller knows how to convert a type T into byte[] and viceversa.
    /// Before being sent in a Hot Rod request,
    /// keys and values must be turned into byte[]. In the other direction, byte[] need to be turned back into objects when a
    /// Hot Rod response is received. Marshallers for key and value must be provided by the user at cache creation.
    /// </summary>
    /// <typeparam name="T">The type handled by the Marshaller</typeparam>
    public abstract class Marshaller<T>
    {
        public abstract byte[] Marshall(T t);
        public abstract T Unmarshall(byte[] buff);
    }

    /// <summary>
    /// An identity Marshaller for byte arrays (no-op serialization)
    /// </summary>
    public class ByteArrayMarshaller : Marshaller<byte[]>
    {
        public static readonly ByteArrayMarshaller Instance = new ByteArrayMarshaller();

        public override byte[] Marshall(byte[] t) => t;
        public override byte[] Unmarshall(byte[] buff) => buff;
    }

    /// <summary>
    /// An untility Marshaller that works on strings using ASCII encoding
    /// by default
    /// </summary>
    public class StringMarshaller : Marshaller<string>
    {

        public static StringMarshaller _ASCII = new StringMarshaller();
        public Encoding Encoding;
        /// <summary>
        /// Creates a StringMarshaller
        /// </summary>
        /// <param name="enc">The chars encoder. ASCII by default</param>
        public StringMarshaller(Encoding enc = null)
        {
            Encoding = (enc == null) ? Encoding.ASCII : Encoding = enc;
        }
        public override byte[] Marshall(string t)
        {
            return t == null ? null : Encoding.GetBytes(t);
        }
        public override string Unmarshall(byte[] buff)
        {
            return buff == null ? null : Encoding.GetString(buff);
        }
    }

    /// <summary>
    /// A string marshaller that wraps values in the protostream WrappedMessage envelope.
    /// Use this for keys in protostream-encoded caches.
    /// </summary>
    public class ProtostreamStringMarshaller : Marshaller<string>
    {
        public override byte[] Marshall(string t)
        {
            return t == null ? null : WrappedMessageHelper.WrapString(t);
        }

        public override string Unmarshall(byte[] buff)
        {
            if (buff == null) return null;
            return WrappedMessageHelper.UnwrapString(buff);
        }
    }

    /// <summary>
    /// A marshaller for Google.Protobuf generated message types that wraps values in the protostream WrappedMessage envelope.
    /// Use this for values in protostream-encoded caches where the entity type is generated from a .proto file.
    /// </summary>
    /// <typeparam name="T">A Google.Protobuf generated message type</typeparam>
    public class ProtobufMarshaller<T> : Marshaller<T> where T : IMessage<T>, new()
    {
        private readonly string _descriptorFullName;
        private readonly MessageParser<T> _parser;

        /// <summary>
        /// Creates a ProtobufMarshaller, automatically deriving the descriptor full name
        /// from the generated protobuf class.
        /// </summary>
        public ProtobufMarshaller()
            : this(new T().Descriptor.FullName)
        {
        }

        /// <summary>
        /// Creates a ProtobufMarshaller for the given protobuf descriptor full name.
        /// </summary>
        /// <param name="descriptorFullName">the protobuf descriptor full name (e.g. "tutorial.Person")</param>
        public ProtobufMarshaller(string descriptorFullName)
        {
            _descriptorFullName = descriptorFullName;
            _parser = new MessageParser<T>(() => new T());
        }

        public override byte[] Marshall(T t)
        {
            if (t == null) return null;
            var size = t.CalculateSize();
            var bytes = new byte[size];
            var cos = new CodedOutputStream(bytes);
            t.WriteTo(cos);
            cos.Flush();
            return WrappedMessageHelper.WrapMessage(bytes, _descriptorFullName);
        }

        public override T Unmarshall(byte[] buff)
        {
            if (buff == null) return default;
            var inner = WrappedMessageHelper.UnwrapBytes(buff);
            if (inner != null)
                return _parser.ParseFrom(inner);
            return _parser.ParseFrom(buff);
        }
    }
}
