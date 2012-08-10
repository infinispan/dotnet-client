using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient
{
    interface IBinaryVersionedValue
    {
        Object getValue();

        void setValue(byte[] val);

    }
}
