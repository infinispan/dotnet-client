using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using Infinispan.DotNetClient.Trans;
using NLog;

namespace Infinispan.DotNetClient.Util.Impl
{
    public class UnsignedNumeric
    {
        /*
         * C#.NET Conversion of Unsigned Numneric class of infinispan.io.UnsignedNumeric.Java written by
         * Manik Surtani.
         */


        /**
         * Reads an int stored in variable-length format.  Reads between one and five bytes.  Smaller values take fewer
         * bytes.  Negative numbers are not supported.
         */
        private static Logger logger = LogManager.GetLogger("UnsignedNumeric");


        public static int ReadUnsignedInt(ITransport trans)
        {

            byte b = trans.getBinaryReader().ReadByte();
            int i = b & 0x7F;
            for (int shift = 7; (b & 0x80) != 0; shift += 7)
            {

                i |= (b & (int)0x7FL) << shift;
            }
            if (logger.IsTraceEnabled)
            {
                logger.Trace(String.Format("Unsigned Int read : " + i));
            }
            return i;
        }


        public static void WriteUnsignedInt(ITransport trans, int i)
        {

            while ((i & ~0x7F) != 0)
            {
                trans.getBinaryWriter().Write((byte)((i & 0x7f) | 0x80));


                i = (int)((uint)i >> 7);

            }


            trans.getBinaryWriter().Write((byte)i);

            if (logger.IsTraceEnabled)
                logger.Trace(String.Format("Unsigned byte written : " + i));

        }




        public static long ReadUnsignedLong(ITransport trans)
        {
            byte b = trans.getBinaryReader().ReadByte();
            long i = b & 0x7F;
            for (int shift = 7; (b & 0x80) != 0; shift += 7)
            {
                b = trans.getBinaryReader().ReadByte();
                i |= (b & 0x7FL) << shift;
            }
            if (logger.IsTraceEnabled)
                logger.Trace(String.Format("Unsigned long read : " + i));
            return i;
        }


        public static void WriteUnsignedLong(ITransport trans, long i)
        {
            while ((i & ~0x7F) != 0)
            {
                trans.getBinaryWriter().Write((byte)((i & 0x7f) | 0x80));

                i = (int)((uint)i >> 7);
            }
            trans.getBinaryWriter().Write((byte)i);
            if (logger.IsTraceEnabled)
                logger.Trace(String.Format("Unsigned Int written : " + i));
        }

        /**
       * Reads an int stored in variable-length format.  Reads between one and five bytes.  Smaller values take fewer
       * bytes.  Negative numbers are not supported.
       */

        public static int ReadUnsignedInt(byte[] bytes, int offset)
        {
            byte b = bytes[offset++];
            int i = b & 0x7F;
            for (int shift = 7; (b & 0x80) != 0; shift += 7)
            {
                b = bytes[offset++];
                i |= (int)((b & 0x7FL) << shift);
            }
            if (logger.IsTraceEnabled)
                logger.Trace(String.Format("Unsigned Int read : " + i));
            return i;
        }


        //*********************************Following methods are not yet used************************

        /**
         * Writes an int in a variable-length format.  Writes between one and five bytes.  Smaller values take fewer bytes.
         * Negative numbers are not supported.
         *
         * @param i int to write
         */
        /*
   public static void writeUnsignedInt(byte[] bytes, int offset, int i) {
      while ((i & ~0x7F) != 0) {
         bytes[offset++] = (byte) ((i & 0x7f) | 0x80);
         i >>>= 7;
      }
      bytes[offset] = (byte) i;
   }


   /**
    * Reads an int stored in variable-length format.  Reads between one and nine bytes.  Smaller values take fewer
    * bytes.  Negative numbers are not supported.
    */
        /*
   public static long readUnsignedLong(byte[] bytes, int offset) {
      byte b = bytes[offset++];
      long i = b & 0x7F;
      for (int shift = 7; (b & 0x80) != 0; shift += 7) {
         b = bytes[offset++];
         i |= (b & 0x7FL) << shift;
      }
      return i;
   }

   /**
    * Writes an int in a variable-length format.  Writes between one and nine bytes.  Smaller values take fewer bytes.
    * Negative numbers are not supported.
    *
    * @param i int to write
    */
        /*
   public static void writeUnsignedLong(byte[] bytes, int offset, long i) {
      while ((i & ~0x7F) != 0) {
         bytes[offset++] = (byte) ((i & 0x7f) | 0x80);
         i >>>= 7;
      }
      bytes[offset] = (byte) i;
   }
        */
    }



}
