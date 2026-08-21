using System;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.Hotrod
{
    public enum ResultType
    {
        Simple,
        Error,
        Integers,
        Bulck,
        Arrays,
        NetError,
        DataError,
        Object,
        String,
        Null,
        NotFound,
        Event
    }

    public enum ResultStatus
    {
        None,
        Loading,
        Completed
    }
}
