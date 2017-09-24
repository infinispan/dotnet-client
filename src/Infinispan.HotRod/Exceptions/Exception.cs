namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Base exception class.
    /// </summary>
    public class Exception : System.ApplicationException
    {
        internal Exception(string message) : base(message)
        {
        }
    }    
}