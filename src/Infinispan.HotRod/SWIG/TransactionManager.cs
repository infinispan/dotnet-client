using System;

namespace Infinispan.HotRod.SWIG
{
    /// <summary>
    /// This class contains method to start commit and rollback a transaction
    /// </summary>
    public interface TransactionManager
    {
        /// <summary>
        /// Begin a transaction
        /// </summary>
        void Begin();
        /// <summary>
        /// Commit a transaction
        /// </summary>
        void Commit();
        /// <summary>
        /// Rollback a transaction
        /// </summary>
        void Rollback();
    }
}
