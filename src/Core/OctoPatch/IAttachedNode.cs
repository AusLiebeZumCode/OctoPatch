namespace OctoPatch
{
    /// <summary>
    /// Base interface for all nodes which are attached to other nodes
    /// </summary>
    public interface IAttachedNode : INode
    {
        /// <summary>
        /// Gets the parent node
        /// </summary>
        INode ParentNode { get; }
    }
}
