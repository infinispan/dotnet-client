using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Infinispan.HotRod
{
    /// <summary>
    /// Hotrod is a binary protocol so the object passed in by the user are being marshalled into a binary format using an implementation of ths interface.
    /// Users can implement this interface and develop their own serializer without using defualt serializer.
    ///  
    /// Author: sunimalr@gmail.com
    /// </summary>
    [Obsolete("Use IMarshaller instread.")]
    public interface ISerializer
    {
        /// <summary>
        /// converts an object ob to a byte array. 
        /// </summary>
        /// <param name="ob">Object which needs to be serialized (convert to byte array)</param>
        /// <returns>Serialized boject as a  byte array</returns>
        byte[] Serialize(Object ob);
            
        //
        /// <summary>
        ///converts a byte array to a object.  
        /// </summary>
        /// <param name="dataArray">byte array that needs to be Deserialized (convert to object)</param>
        /// <returns>Deserialized object</returns>
        Object Deserialize(byte[] dataArray);
    }
}
