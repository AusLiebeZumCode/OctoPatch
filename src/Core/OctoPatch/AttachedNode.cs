using System;

namespace OctoPatch
{
    /// <summary>
    /// Special node to be attached directly to a specified node
    /// </summary>
    public abstract class AttachedNode<T> : Node<IConfiguration, IEnvironment> where T : INode
    {
        /// <summary>
        /// Reference to the parent node
        /// </summary>
        protected T ParentNode { get; }

        protected AttachedNode(Guid nodeId, T parentNode) : base(nodeId)
        {
            ParentNode = parentNode;
        }
    }
}
