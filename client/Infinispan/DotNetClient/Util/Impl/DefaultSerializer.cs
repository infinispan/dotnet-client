using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NLog;

namespace Infinispan.DotNetClient.Util.Impl
{
    /// <summary>
    /// This is the default serializer.
    /// Handles serialization and deserialization of objects.
    ///This allows the user to convert and high level data type to a byte array.
    /// 
    /// Author: sunimalr@gmail.com
    /// 
    /// </summary>
    public class DefaultSerializer : ISerializer
    {
        //TODO: Improve Serialization Efficiency
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


        private byte[] Iternal_serialize(Object ob)
        {
            this.binaryFormatter.Serialize(memoryStreamIn, ob);
            if (logger.IsTraceEnabled)
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
        public byte[] Serialize(Object ob)
        {
            return new DefaultSerializer().Iternal_serialize(ob);
        }


        /// <summary>
        ///converts a byte array to a object. 
        /// </summary>
        /// <param name="dataArray"></param>
        /// <returns>Deserialized object from byte array if dataarray is not null, else null will be returned.</returns>
        public Object Deserialize(byte[] dataArray)
        {
            if (dataArray == null)
            {
                return null;
            }
            else
            {
                MemoryStream m = new MemoryStream(dataArray);
                Object o = (Object)binaryFormatter.Deserialize(m);
                if (logger.IsTraceEnabled)
                    logger.Trace("Deserialized : " + o.ToString());
                return o;
            }
        }
    }
}
