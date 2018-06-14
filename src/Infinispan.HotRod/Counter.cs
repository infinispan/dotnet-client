using System;

namespace Infinispan.HotRod
{
    /// <summary>
    /// This class collects all the common methods for a counter
    /// </summary>
    public interface Counter
    {
        /// <summary>
        /// Get the counter name
        /// </summary>
        /// <returns>counter name</returns>
        string GetName();
        /// <summary>
        /// Get the configuration
        /// </summary>
        /// <returns>the configuration</returns>
        ICounterConfiguration GetConfiguration();
        /// <summary>
        /// Reset a counter to its initial values
        /// </summary>
        void Reset();
        /// <summary>
        /// Remove the counter.
        /// The counter will be recreated if accessed after a remove
        /// </summary>
        void Remove();
        /// <summary>
        /// Add a listener to the counter
        /// </summary>
        /// <param name="listener">the counter listener</param>
        /// <returns> the handler to the registered listener. Needed by the RemoveListener method</returns>
        object AddListener(Event.CounterListener listener);
        /// <summary>
        /// Remove the listener. The counter will be recreated if accessed after a remove.
        /// </summary>
        /// <param name="handler"> the handler to the listener returned by the AddListener operation</param>
        void RemoveListener(object handler);
    }

    /// <summary>
    /// The strong counter provides higher consistency. Its value is known during the update and its updates are applied atomically. This allows to set boundaries and provides conditional operation (as compare-and-set). 
    /// </summary>
    public interface StrongCounter : Counter
    {
        /// <summary>
        /// get the current value
        /// </summary>
        /// <returns>the current value</returns>
        int GetValue();
        /// <summary>
        /// Add a delta quantity and return the value
        /// </summary>
        /// <param name="delta">quantity to be added</param>
        /// <returns>the new value</returns>
        int AddAndGet(int delta);
        /// <summary>
        /// Increment by 1 and return the value
        /// </summary>
        /// <returns>the new value</returns>
        int IncrementAndGet();
        /// <summary>
        /// Decrement by 1 and return the value
        /// </summary>
        /// <returns>the new value</returns>
        int DecrementAndGet();
        /// <summary>
        /// If the current value equals expect param then set the value to the given update
        /// </summary>
        /// <param name="expect">the expected current value</param>
        /// <param name="update">the new value</param>
        /// <returns>the previous value</returns>
        int CompareAndSwap(int expect, int update);
        /// If the current value equals expect param then set the value to the given update
        /// </summary>
        /// <param name="expect">the expected current value</param>
        /// <param name="update">the new value</param>
        /// <returns>true if the old value equals expect</returns>
        bool CompareAndSet(int expect, int update);
    }

    /// <summary>
    ///The weak counter provides eventual consistency and its value is not known during updates. It provides faster writes when comparing with the strong counter. 
    /// </summary>
    public interface WeakCounter : Counter
    {
        /// <summary>
        /// Return the current value
        /// </summary>
        /// <returns>the current value. this could not be the most updated</returns>
        int GetValue();
        /// <summary>
        /// Increment counter's value by 1
        /// </summary>
        void Increment();
        /// <summary>
        /// Decrement counter's value by 1
        /// </summary>
        void Decrement();
        /// <summary>
        /// Increment counter's value by delta
        /// </summary>
        /// <param name="delta">the increment value</param>
        void Add(int delta);
    }
}
