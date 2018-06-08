namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Exception used to indicate that a counter reach the lower bound
    /// </summary>
    public class CounterLowerBoundException : Exception
    {
        internal CounterLowerBoundException(string message) : base(message)
        {
        }
    }
}
