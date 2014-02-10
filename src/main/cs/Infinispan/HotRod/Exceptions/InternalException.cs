namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Represents an internal exception.
    /// </summary>
    public class InternalException : HotRodClientException
    {
        internal InternalException(string message) : base(message)
        {
        }
    }    
}