using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinispan.HotRod.Event
{
    /// <summary>
    /// Counter listener class.
    /// Object of this class can be registered as listener for counter event
    /// </summary>
    public class CounterListener
    {
        internal class CounterListenerImpl : Infinispan.HotRod.SWIGGen.CounterListener
        {
            Action<CounterEvent> action;
            public CounterListenerImpl(string counterName, Action<CounterEvent> al) : base(counterName)
            {
                this.action = al;
            }
            public override void onUpdate(Infinispan.HotRod.SWIGGen.CounterEvent entry)
            {
                action(new CounterEvent(entry));
            }
        }
        internal CounterListenerImpl cli;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <param name="al">action to execute on events</param>
        public CounterListener(string counterName, Action<CounterEvent> al)
        {
            cli = new CounterListenerImpl(counterName, al);
        }
    }
}
