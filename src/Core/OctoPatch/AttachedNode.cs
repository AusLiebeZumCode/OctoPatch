using System;

namespace OctoPatch
{
    /// <summary>
    /// Special node to be attached directly to a specified node
    /// </summary>
    /// <typeparam name="TConfiguration">configuration type</typeparam>
    /// <typeparam name="TEnvironment">environment type</typeparam>
    /// <typeparam name="TParent">Type of parent node</typeparam>
    public abstract class AttachedNode<TConfiguration, TEnvironment, TParent> : Node<TConfiguration, TEnvironment>, IAttachedNode
        where TConfiguration : IConfiguration
        where TEnvironment : IEnvironment
        where TParent : INode
    {
        /// <summary>
        /// Reference to the parent node
        /// </summary>
        protected TParent ParentNode { get; }

        protected AttachedNode(Guid nodeId, TParent parentNode) : base(nodeId)
        {
            if (parentNode == null)
            {
                throw new ArgumentNullException(nameof(parentNode));
            }

            ParentNode = (TParent)parentNode;
        }
    }
}
