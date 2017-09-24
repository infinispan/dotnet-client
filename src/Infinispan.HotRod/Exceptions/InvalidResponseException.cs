namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Used to indicated situations when an invalid respose is received
    /// from the HotRod server.
    /// </summary>
    public class InvalidResponseException : HotRodClientException
    {
        internal InvalidResponseException(string message) : base(message)
        {
        }
    }    
}