namespace Org.Infinispan.Query.Remote.Client
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class QueryRequest
    {
        /// <summary>
        /// This property will be soon deprecated, use QueryString directly
        /// </summary>
        public string JpqlString
        {
            get { return queryString_; }
            set
            {
                queryString_ = value;
            }
        }
    }
}