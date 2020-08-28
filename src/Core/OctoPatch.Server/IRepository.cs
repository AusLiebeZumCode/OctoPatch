using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;

namespace OctoPatch.Server
{
    /// <summary>
    /// Repository for some plugin driven types
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Returns all discovered messages
        /// </summary>
        /// <returns>all messages</returns>
        IEnumerable<TypeDescription> GetMessageDescriptions();

        /// <summary>
        /// Returns all discovered nodes
        /// </summary>
        /// <returns>all nodes</returns>
        IEnumerable<NodeDescription> GetNodeDescriptions();

        /// <summary>
        /// Generates a new node for the given node guid
        /// </summary>
        /// <param name="key">unique key of the node description</param>
        /// <param name="nodeId">id of the new node</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="pluginId">id of related plugin</param>
        /// <returns>new node instance</returns>
        Task<INode> CreateNode(Guid pluginId, string key, Guid nodeId, CancellationToken cancellationToken);
    }
}
