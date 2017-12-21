using Infinispan.HotRod.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.HotRod.Tests
{
    [TestFixture]
    public class CompatibilityMarshallerTest 
    {
        private CompatibilityMarshaller marshaller;

        [SetUp]
        public void Before()
        {
            marshaller = new CompatibilityMarshaller();
        }

        [Test]
        public void MarshallerNonString()
        {
            HotRodClientException ex = Assert.Throws<HotRodClientException>
                (delegate
                 {
                     marshaller.ObjectToByteBuffer(12345);
                 });
            Assert.That(ex.Message, Is.EqualTo("Cannot serialize non-string object: 12345."));
        }

        [Test]
        public void UnmarshallerUnknownVersion()
        {
            HotRodClientException ex = Assert.Throws<HotRodClientException>
                (delegate
                 {
                     marshaller.ObjectFromByteBuffer(new byte[] {4});
                 });
            Assert.That(ex.Message, Is.EqualTo("Unknown compatibility serialization version: 4."));
        }

        [Test]
        public void UnmarshallerNonString()
        {
            HotRodClientException ex = Assert.Throws<HotRodClientException>
                (delegate
                 {
                     marshaller.ObjectFromByteBuffer(new byte[] {3, 0xff});
                 });
            Assert.That(ex.Message, Is.EqualTo("Cannot deserialize non-string (type 255)."));
        }

        [Test]
        public void MarshallerNull()
        {
            int header = 2;
            byte[] ba = marshaller.ObjectToByteBuffer(null);
            Assert.AreEqual(header, ba.Length);
            //version
            Assert.AreEqual(3, ba[0]);
            //null
            Assert.AreEqual(0x01, ba[1]);
        }

        [Test]
        public void MarshallerEmpty()
        {
            int header = 2;
            byte[] ba = marshaller.ObjectToByteBuffer("");
            Assert.AreEqual(header, ba.Length);
            //version
            Assert.AreEqual(3, ba[0]);
            //null
            Assert.AreEqual(0x3d, ba[1]);
        }

        [Test]
        public void MarshallerSmallLowerBound()
        {
            int header = 3;
            int length = 1;
            string input = GenerateString(length);
            byte[] utf = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] ba = marshaller.ObjectToByteBuffer(input);
            Assert.AreEqual(header + utf.Length, ba.Length);

            //version
            Assert.AreEqual(3, ba[0]);
            //small string
            Assert.AreEqual(0x3e, ba[1]);
            //size
            Assert.AreEqual(0x01, ba[2]);

            CheckEqual(utf, ba, 3);
        }

        [Test]
        public void MarshallerSmallUpperBound()
        {
            int header = 3;
            int length = 256;
            string input = GenerateString(length);
            byte[] utf = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] ba = marshaller.ObjectToByteBuffer(input);
            Assert.AreEqual(header + utf.Length, ba.Length);

            //version
            Assert.AreEqual(3, ba[0]);
            //small string
            Assert.AreEqual(0x3e, ba[1]);
            //size
            Assert.AreEqual(0x00, ba[2]);
            CheckEqual(utf, ba, 3);
        }

        [Test]
        public void MarshallerMediumLowerBound()
        {
            int header = 4;
            int length = 257;
            string input = GenerateString(length);
            byte[] utf = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] ba = marshaller.ObjectToByteBuffer(input);
            Assert.AreEqual(header + utf.Length, ba.Length);

            //version
            Assert.AreEqual(3, ba[0]);
            //medium string
            Assert.AreEqual(0x3f, ba[1]);
            //size
            Assert.AreEqual(0x01, ba[2]);
            Assert.AreEqual(0x01, ba[3]);
            CheckEqual(utf, ba, 4);
        }

        [Test]
        public void MarshallerMediumUpperBound()
        {
            int header = 4;
            int length = 65536;
            string input = GenerateString(length);
            byte[] utf = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] ba = marshaller.ObjectToByteBuffer(input);
            Assert.AreEqual(header + utf.Length, ba.Length);

            //version
            Assert.AreEqual(3, ba[0]);
            //medium string
            Assert.AreEqual(0x3f, ba[1]);
            //size
            Assert.AreEqual(0x00, ba[2]);
            Assert.AreEqual(0x00, ba[3]);
            CheckEqual(utf, ba, 4);
        }

        [Test]
        public void MarshallerLargeLowerBound()
        {
            int header = 6;
            int length = 65537;
            string input = GenerateString(length);
            byte[] utf = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] ba = marshaller.ObjectToByteBuffer(input);
            Assert.AreEqual(header + utf.Length, ba.Length);

            //version
            Assert.AreEqual(3, ba[0]);
            //medium string
            Assert.AreEqual(0x40, ba[1]);
            //size
            Assert.AreEqual(0x00, ba[2]);
            Assert.AreEqual(0x01, ba[3]);
            Assert.AreEqual(0x00, ba[4]);
            Assert.AreEqual(0x01, ba[5]);

            CheckEqual(utf, ba, 6);
        }

        [Test]
        public void UnmarshallerNull()
        {
            byte[] ba = new byte[] {3, 0x01};
            Assert.AreEqual(null, marshaller.ObjectFromByteBuffer(ba));
        }

        [Test]
        public void UnmarshallerEmpty()
        {
            byte[] ba = new byte[] {3, 0x3d};
            Assert.AreEqual("", marshaller.ObjectFromByteBuffer(ba));
        }

        [Test]
        public void UnmarshallerSmallLowerBound()
        {
            int length = 1;
            string input = GenerateString(length);

            List<byte> bytes = new List<byte>();
            bytes.Add(3);
            bytes.Add(0x3e);
            bytes.Add(0x01);
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(input));
            Assert.AreEqual(input, marshaller.ObjectFromByteBuffer(bytes.ToArray()));
        }

        [Test]
        public void UnmarshallerSmallUpperBound()
        {
            int length = 256;
            string input = GenerateString(length);

            List<byte> bytes = new List<byte>();
            bytes.Add(3);
            bytes.Add(0x3e);
            bytes.Add(0x00);
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(input));
            Assert.AreEqual(input, marshaller.ObjectFromByteBuffer(bytes.ToArray()));
        }

        [Test]
        public void UnmarshallerMediumLowerBound()
        {
            int length = 257;
            string input = GenerateString(length);

            List<byte> bytes = new List<byte>();
            bytes.Add(3);
            bytes.Add(0x3f);
            bytes.Add(0x01);
            bytes.Add(0x01);
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(input));
            Assert.AreEqual(input, marshaller.ObjectFromByteBuffer(bytes.ToArray()));
        }

        [Test]
        public void UnmarshallerMediumUpperBound()
        {
            int length = 65536;
            string input = GenerateString(length);

            List<byte> bytes = new List<byte>();
            bytes.Add(3);
            bytes.Add(0x3f);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(input));
            Assert.AreEqual(input, marshaller.ObjectFromByteBuffer(bytes.ToArray()));
        }

        [Test]
        public void UnmarshallerLargeLowerBound()
        {
            int length = 65537;
            string input = GenerateString(length);

            List<byte> bytes = new List<byte>();
            bytes.Add(3);
            bytes.Add(0x40);
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(input));
            Assert.AreEqual(input, marshaller.ObjectFromByteBuffer(bytes.ToArray()));
        }

        private string GenerateString(int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
                {
                    builder.Append((char) (i % 10));
                }
            return builder.ToString();
        }

        private void CheckEqual(byte[] expected, byte[] actual, int startIndex)
        {
            Assert.GreaterOrEqual(startIndex, 0);
            Assert.AreEqual(expected.Length, actual.Length - startIndex);
            for (int i = 0; i < expected.Length; i++)
                {
                    Assert.AreEqual(expected[i], actual[i + startIndex]);
                }
        }
    }
}
