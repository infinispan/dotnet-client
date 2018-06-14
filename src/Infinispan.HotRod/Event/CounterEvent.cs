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
        /// Counter name
        /// </summary>
        public string GetCounterName
        {
            get { return swigEvent.counterName; }
        }

        /// <summary>
        /// The previous value
        /// </summary>
        public long OldValue
        {
            get { return swigEvent.oldValue; }
        }

        /// <summary>
        /// The previous state
        /// </summary>
        public CounterState OldState
        {
            get { return (CounterState)swigEvent.oldState; }
        }

        /// <summary>
        /// The counter value
        /// </summary>
        public long NewValue
        {
            get { return swigEvent.newValue; }
        }

        /// <summary>
        /// The counter state
        /// </summary>
        public CounterState NewState
        {
            get { return (CounterState)swigEvent.newState; }
        }
    }
}


