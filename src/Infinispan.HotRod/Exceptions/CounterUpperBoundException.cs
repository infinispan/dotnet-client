namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Exception used to indicate that a counter reached the upper bound
    /// </summary>
    public class CounterUpperBoundException : Exception
    {
        internal CounterUpperBoundException(string message) : base(message)
        {
        }
    }
}
