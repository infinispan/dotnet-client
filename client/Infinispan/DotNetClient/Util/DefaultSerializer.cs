using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NLog;


namespace Infinispan.DotNetClient.Util
{
    /// <summary>
    /// This is the default serializer.
    /// Handles serialization and deserialization of objects.
    ///This allows the user to convert and high level data type to a byte array.
    /// 
    /// Author: sunimalr@gmail.com
    /// 
    /// </summary>
    public class DefaultSerializer : Serializer
    {

        private static Logger logger;
        private BinaryFormatter binaryFormatter;
        private MemoryStream memoryStreamIn = new MemoryStream();
        private MemoryStream memoryStreamOut = new MemoryStream();

        /// <summary>
        /// The default serializer to be used if the user doesn't use a custom serialzer
        /// </summary>
        public DefaultSerializer()
        {
            this.binaryFormatter = new BinaryFormatter();
            this.memoryStreamIn = new MemoryStream();
            this.memoryStreamOut = new MemoryStream();
            logger = LogManager.GetLogger("Serializer");
        }

        
        private byte[] iternal_serialize(Object ob)
        {
            this.binaryFormatter.Serialize(memoryStreamIn, ob);
            logger.Trace("Serialized : " + ob.ToString());
            byte[] returnArr = memoryStreamIn.ToArray();
            memoryStreamIn.Flush();
            return returnArr;
        }

        /// <summary>
        /// converts an object ob to a byte array.
        /// </summary>
        /// <param name="ob"></param>
        /// <returns>Serialzed object as a byte array</returns>
        public byte[] serialize(Object ob)
        {
            return new DefaultSerializer().iternal_serialize(ob);// returnArr;
        }

        
        /// <summary>
        ///converts a byte array to a object. 
        /// </summary>
        /// <param name="dataArray"></param>
        /// <returns>Deserialized object from byte array</returns>
        public Object deserialize(byte[] dataArray)
        {
            this.memoryStreamOut.Write(dataArray, 0, dataArray.Length);
            this.memoryStreamOut.Seek(0, SeekOrigin.Begin);
            Object o = (Object)binaryFormatter.Deserialize(memoryStreamOut);
            memoryStreamOut.Flush();
            logger.Trace("Deserialized : " + o.ToString());
            return o;
        }
    }
}
