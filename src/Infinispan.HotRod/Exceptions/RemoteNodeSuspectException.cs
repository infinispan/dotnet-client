namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Used to indicate that a remote HotRod server is not reachable.
    /// </summary>
    public class RemoteNodeSuspectException : HotRodClientException
    {
        internal RemoteNodeSuspectException(string message) : base(message)
        {
        }
    }    
}