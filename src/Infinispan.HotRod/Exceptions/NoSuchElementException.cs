namespace Infinispan.HotRod.Exceptions
{
    /// <summary>
    ///   Exception raised if the requested object doesn't exists
    ///   The connection pool raises this exec if is exhausted and configured
    ///   with EXCEPTION
    /// </summary>
    public class NoSuchElementException : Exception
    {
        internal NoSuchElementException(string message) : base(message)
        {
        }
    }
}
