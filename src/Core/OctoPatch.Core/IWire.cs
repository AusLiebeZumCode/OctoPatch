namespace OctoPatch.Core
{
    /// <summary>
    /// Interface for a node wire
    /// </summary>
    public interface IWire
    {
        /// <summary>
        /// Gets the wired input connector
        /// </summary>
        IInputConnector InputConnector { get; }

        /// <summary>
        /// Gets the wired output connector
        /// </summary>
        IOutputConnector OutputConnector { get; }
    }
}
