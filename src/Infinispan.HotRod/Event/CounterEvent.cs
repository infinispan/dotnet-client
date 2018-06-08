namespace Infinispan.HotRod.Event
{
    /// <summary>
    /// Counter state types enumeration
    /// </summary>
    public enum CounterState
    {
        VALID,
        LOWER_BOUND_REACHED,
        UPPER_BOUND_REACHED
    }

    /// <summary>
    /// The counter event representation
    /// </summary>
    public class CounterEvent
    {
        private Infinispan.HotRod.SWIGGen.CounterEvent swigEvent;
        internal CounterEvent(Infinispan.HotRod.SWIGGen.CounterEvent e)
        {
            swigEvent = e;
        }
        /// <summary>
        /// counter name
        /// </summary>
        public string GetCounterName
        {
            get { return swigEvent.counterName; }
        }

        /// <summary>
        /// the previous value
        /// </summary>
        public long oldValue
        {
            get { return swigEvent.oldValue; }
        }

        /// <summary>
        /// the previous state
        /// </summary>
        public CounterState oldState
        {
            get { return (CounterState)swigEvent.oldState; }
        }

        /// <summary>
        /// the counter value
        /// </summary>
        public long newValue
        {
            get { return swigEvent.newValue; }
        }

        /// <summary>
        /// the counter state
        /// </summary>
        public CounterState newState
        {
            get { return (CounterState)swigEvent.newState; }
        }
    }
}


