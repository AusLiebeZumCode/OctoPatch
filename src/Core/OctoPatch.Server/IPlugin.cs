using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Core;

namespace OctoPatch.Server
{
    /// <summary>
    /// Interface to describe a plugin
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the optional description for the plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the current version of this plugin.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Lists up all containing node descriptions.
        /// </summary>
        /// <returns>List of node descriptions</returns>
        IEnumerable<NodeDescription> GetNodeDescriptions();

        /// <summary>
        /// Lists up all containing message descriptions.
        /// </summary>
        /// <returns>List of message descriptions</returns>
        IEnumerable<MessageDescription> GetMessageDescriptions();

        /// <summary>
        /// Method to generate a new instance of the node with the given node guid.
        /// </summary>
        /// <param name="nodeDescriptionGuid">node description guid</param>
        /// <param name="nodeId">id of the new node</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>new instance of a node with the given guid</returns>
        Task<INode> CreateNode(Guid nodeDescriptionGuid, Guid nodeId, CancellationToken cancellationToken);

    }
}
