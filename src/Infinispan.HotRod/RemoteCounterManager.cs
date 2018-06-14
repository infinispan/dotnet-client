using System;

namespace Infinispan.HotRod
{
    /// <summary>
    /// Manager for all the counter-related operations
    /// </summary>
    public interface RemoteCounterManager
    {
        /// <summary>
        /// Get a strong counter
        /// </summary>
        /// <param name="name">the counter's name</param>
        /// <returns>the requested counter</returns>
        StrongCounter GetStrongCounter(string name);
        /// <summary>
        /// Get a weak counter
        /// </summary>
        /// <param name="name">the counter's name</param>
        /// <returns>the requested counter</returns>
        WeakCounter GetWeakCounter(string name);
        /// <summary>
        /// Define a counter on the server
        /// </summary>
        /// <param name="name">name of the counter</param>
        /// <param name="configuration">counter's conf properties</param>
        /// <returns></returns>
        bool DefineCounter(string name, ICounterConfiguration configuration);
        /// <summary>
        /// Check if a counter is defined
        /// </summary>
        /// <param name="name">name of the counter</param>
        /// <returns>true if defined</returns>
        bool IsDefined(string name);
        /// <summary>
        /// Get the configuration for a counter
        /// </summary>
        /// <param name="name">counter's name</param>
        /// <returns>return the configuration</returns>
        ICounterConfiguration GetConfiguration(string name);
        /// <summary>
        /// Remove a counter by name. This is equivalent to call <see cref="M:Counter.Remove()"/>
        /// The counter will be recreated if accessed after a remove
        /// </summary>
        /// <param name="counterName">counter's name</param>
        void Remove(string counterName);
    }
}
