namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Used to indicated that a TCP communication exception occurred when
    /// communicating with the HotRod server.
    /// </summary>
    public class TransportException : HotRodClientException
    {
        internal TransportException(string message) : base(message)
        {
        }
    }    
}