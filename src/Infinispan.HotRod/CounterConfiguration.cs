using System;

namespace Infinispan.HotRod
{
    /// <summary>
    /// An interface for the counter configuration
    /// </summary>
    public interface ICounterConfiguration
    {
        /// <summary>
        /// the initial value
        /// </summary>
        /// <returns></returns>
        int GetInitialValue();
        /// <summary>
        /// The upper bound
        /// </summary>
        /// <returns></returns>
        int GetUpperBound();
        /// <summary>
        /// The lower bound
        /// </summary>
        /// <returns></returns>
        int GetLowerBound();
        /// <summary>
        /// The counter type
        /// </summary>
        /// <returns></returns>
        CounterType GetType();
        /// <summary>
        /// Get concurrency level
        /// </summary>
        /// <returns></returns>
        int GetConcurrencyLevel();
        /// <summary>
        /// The storage type
        /// </summary>
        /// <returns></returns>
        Storage GetStorage();
        /// <summary>
        /// Return a string representation of the configuration
        /// </summary>
        /// <returns></returns>
        string ToString();
    }

    /// <summary>
    /// The public implementation of the counter configuration
    /// </summary>
    public class CounterConfiguration : ICounterConfiguration
    {
        int initialValue;
        int upperBound;
        int lowerBound;
        int concurrencyLevel;
        CounterType counterType;
        Storage storage;

        public CounterConfiguration(int initialValue, int lowerBound, int upperBound, int concurrencyLevel,
            CounterType counterType, Storage storage)
        {
            this.initialValue = initialValue;
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
            this.concurrencyLevel = concurrencyLevel;
            this.counterType = counterType;
            this.storage = storage;
        }
        /// <summary>
        /// the initial value
        /// </summary>
        /// <returns></returns>
        public int GetInitialValue()
        {
            return initialValue;
        }
        /// <summary>
        /// The upper bound
        /// </summary>
        /// <returns></returns>
        public int GetUpperBound()
        {
            return upperBound;
        }
        /// <summary>
        /// The lower bound
        /// </summary>
        /// <returns></returns>
        public int GetLowerBound()
        {
            return lowerBound;
        }
        /// <summary>
        /// The counter type
        /// </summary>
        /// <returns></returns>
        public CounterType GetType()
        {
            return counterType;
        }
        /// <summary>
        /// Get concurrency level
        /// </summary>
        /// <returns></returns>
        public int GetConcurrencyLevel()
        {
            return concurrencyLevel;
        }
        /// <summary>
        /// The storage type
        /// </summary>
        /// <returns></returns>
        public Storage GetStorage()
        {
            return storage;
        }
        /// <summary>
        /// Return a string representation of the configuration
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            return "CounterConfiguration{" +
        "initialValue=" + initialValue +
        ", upperBound=" + upperBound +
        ", lowerBound=" + lowerBound +
        ", concurrencyLevel=" + concurrencyLevel +
        ", type=" + counterType +
        ", storage=" + storage +
        "}";
        }
    }

}
