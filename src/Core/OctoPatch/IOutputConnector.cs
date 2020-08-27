namespace OctoPatch
{
    /// <summary>
    /// Interface for an output connector
    /// </summary>
    public interface IOutputConnector : IConnector
    {
        /// <summary>
        /// Returns the description for this output
        /// </summary>
        OutputDescription OutputDescription { get; }
    }
}
