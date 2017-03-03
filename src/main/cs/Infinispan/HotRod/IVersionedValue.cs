using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.HotRod
{
    /**
       <summary>
       A value and the version associated with this value.
       </summary>
    */
    public interface IVersionedValue<V>
    {
        /**
           <summary>
           The actual value.
           </summary>
        */
        V GetValue();
        
        /**
           <summary>
           Returns the versioned associated with the value.
           </summary>
        */
        ulong GetVersion();
    }
}
