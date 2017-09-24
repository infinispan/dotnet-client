namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Used to indicated that the operation invoked is not implemented yet or unsupported.
    /// </summary>
    public class UnsupportedOperationException : HotRodClientException
    {
        internal UnsupportedOperationException(string message) : base(message)
        {
        }
    }    
}