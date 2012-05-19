using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Infinispan.DotNetClient.Util
{
    public interface Serializer
    {
    /*
     * This is the serializer interface.
     * Users can implement this interface and develop their own serializer without using defualt serializer.
     * 
     * Author: sunimalr@gmail.com
     * 
     */
        
        //converts an object ob to a byte array. 
        byte[] serialize(Object ob);
            
        //converts a byte array to a object. 
        Object deserialize(byte[] dataArray);
    }
}
