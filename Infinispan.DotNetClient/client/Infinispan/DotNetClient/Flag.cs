using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient
{
    /*
    * 
    * Flag element of the header.
    *
    * Author: sunimalr@gmail.com
    *      
    */

    public class  Flag
    {
        public const byte FORCE_RETURN_VALUE = 0x0001;
        private int flagInt;

        public Flag(int flagInt)
        {
            this.flagInt = flagInt;
        }

        public int getFlagInt()
        {
            return flagInt;
        }

    }
}
