namespace OctoPatch
{
    /// <summary>
    /// Interface for a wire adapter
    /// </summary>
    public interface IAdapter
    {
        /// <summary>
        /// Single input connector
        /// </summary>
        IInputConnector Input { get; }

        /// <summary>
        /// Single output connector
        /// </summary>
        IOutputConnector Output { get; }
    }
}
