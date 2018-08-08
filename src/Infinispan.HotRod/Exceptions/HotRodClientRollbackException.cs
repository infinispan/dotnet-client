namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Used to indicated that a commit operation actually rolled back
    /// </summary>
    public class HotRodClientRollbackException : HotRodClientException
    {
        internal HotRodClientRollbackException(string message) : base(message)
        {
        }
    }    
}
