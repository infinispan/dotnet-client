namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Exception used to indicate exceptions using the HotRod client.
    /// </summary>
    public class HotRodClientException : Exception
    {
        internal HotRodClientException(string message) : base(message)
        {
        }
    }    
}