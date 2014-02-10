namespace Infinispan.HotRod.Config
{
    /// <summary>
    ///   Common builder interface.
    /// </summary>
    public interface IBuilder<T>
    {
        /// <summary>
        ///   Validate builder state.
        /// </summary>
        void Validate();

        /// <summary>
        ///   Create the instance.
        /// </summary>
        T Create();

        /// <summary>
        ///   Read the builder state from the object supplied.
        /// </summary>
        ///
        /// <param name="bean">the object holding the state</param>
        IBuilder<T> Read(T bean);
    }
}