using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinispan.DotNetClient.Hotrod
{
    interface IVersionedValue
    {
        Object getValue();

        long getVersion();
    }
}
