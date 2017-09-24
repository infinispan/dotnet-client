using System;

namespace Infinispan.HotRod.SWIG
{
    internal interface ServerConfiguration
    {
        string getHost();
        int getPort();
    }
}