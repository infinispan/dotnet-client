using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Infinispan.DotNetClient.Protocol
{
    /*
     * 
     * This abstract class defines all the constants used in hotrod protocol including supported response and request codes
     * as defined in https://docs.jboss.org/author/display/ISPN/Hot+Rod+Protocol+-+Version+1.1
     * 
     * Author: sunimalr@gmail.com
     *      
     */

    public abstract class HotRodConstants
    {
        //Note: Hot Rod .NET client library only supports Hot Rod Protocol version 1.1

        public const byte REQUEST_MAGIC = 0xA0;
        public const byte RESPONSE_MAGIC = 0xA1;
        public const byte VERSION_10 = 10;
        public const byte VERSION_11 = 11;

        //request codes
        public const byte PUT_REQUEST = 0x01;
        public const byte GET_REQUEST = 0x03;
        public const byte PUT_IF_ABSENT_REQUEST = 0x05;
        public const byte REPLACE_REQUEST = 0x07;
        public const byte REPLACE_IF_UNMODIFIED_REQUEST = 0x09;
        public const byte REMOVE_REQUEST = 0x0B;
        public const byte REMOVE_IF_UNMODIFIED_REQUEST = 0x0D;
        public const byte CONTAINS_KEY_REQUEST = 0x0F;
        public const byte GET_WITH_VERSION = 0x11;
        public const byte CLEAR_REQUEST = 0x13;
        public const byte STATS_REQUEST = 0x15;
        public const byte PING_REQUEST = 0x17;
        public const byte BULK_GET_REQUEST = 0x19;


        //response codes
        public const byte PUT_RESPONSE = 0x02;
        public const byte GET_RESPONSE = 0x04;
        public const byte PUT_IF_ABSENT_RESPONSE = 0x06;
        public const byte REPLACE_RESPONSE = 0x08;
        public const byte REPLACE_IF_UNMODIFIED_RESPONSE = 0x0A;
        public const byte REMOVE_RESPONSE = 0x0C;
        public const byte REMOVE_IF_UNMODIFIED_RESPONSE = 0x0E;
        public const byte CONTAINS_KEY_RESPONSE = 0x10;
        public const byte GET_WITH_VERSION_RESPONSE = 0x12;
        public const byte CLEAR_RESPONSE = 0x14;
        public const byte STATS_RESPONSE = 0x16;
        public const byte PING_RESPONSE = 0x18;
        public const byte BULK_GET_RESPONSE = 0x1A;
        public const byte ERROR_RESPONSE = 0x50;

        //response status codes
        public const byte NO_ERROR_STATUS = 0x00;
        public const byte INVALID_MAGIC_OR_MESSAGE_ID_STATUS = 0x81;
        public const byte REQUEST_PARSING_ERROR_STATUS = 0x84;
        public const byte NOT_PUT_REMOVED_REPLACED_STATUS = 0x01;
        public const byte UNKNOWN_COMMAND_STATUS = 0x82;
        public const byte SERVER_ERROR_STATUS = 0x85;
        public const byte KEY_DOES_NOT_EXIST_STATUS = 0x02;
        public const byte UNKNOWN_VERSION_STATUS = 0x83;
        public const byte COMMAND_TIMEOUT_STATUS = 0x86;
        public const byte CLIENT_INTELLIGENCE_BASIC = 0x01;
        public const byte CLIENT_INTELLIGENCE_TOPOLOGY_AWARE = 0x02;
        public const byte CLIENT_INTELLIGENCE_HASH_DISTRIBUTION_AWARE = 0x03;
        public static byte[] DEFAULT_CACHE_NAME_BYTES = new byte[] { };

    }
}
