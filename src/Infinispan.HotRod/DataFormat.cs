namespace Infinispan.HotRod
{
    /// <summary>
    /// 
    /// </summary>
    public class DataFormat 
    {
        internal SWIGGen.ByteArrayDataFormat df;
        /// <summary>
        /// 
        /// </summary>
        public DataFormat()
        {
            df = new SWIGGen.ByteArrayDataFormat();
        }

        /// <summary>
        /// Marshaller to be used for key and value
        /// </summary>
        public IMarshaller Marshaller;

        /// <summary>
        /// Marshaller to be used for Exec arguments
        /// </summary>
        public IMarshaller ArgMarshaller;

        /// <summary>
        /// Mediatype for key
        /// </summary>
        public string KeyMediaType
        {
            get
            {
                return df.keyMediaType.typeSubtype;
            }

            set
            {
                df.keyMediaType.typeSubtype = value;
            }
        }

        /// <summary>
        /// Mediatype for value
        /// </summary>
        public string ValueMediaType
        {
            get
            {
                return df.valueMediaType.typeSubtype;
            }

            set
            {
                df.valueMediaType.typeSubtype = value;
            }
        }
    }
}
