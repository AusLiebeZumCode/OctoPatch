namespace OctoPatch
{
    /// <summary>
    /// Interface for all nodes which splits up complex types
    /// </summary>
    public interface ISplitterNode : INode
    {
        /// <summary>
        /// Reference to the related connector
        /// </summary>
        IOutputConnector Connector { get; }
    }
}
