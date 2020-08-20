using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Core;

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
        IEnumerable<MessageDescription> GetMessageDescriptions();

        /// <summary>
        /// Returns all discovered nodes
        /// </summary>
        /// <returns>all nodes</returns>
        IEnumerable<NodeDescription> GetNodeDescriptions();

        /// <summary>
        /// Generates a new node for the given node guid
        /// </summary>
        /// <param name="nodeDescriptionGuid">guid of node to generate an instance of</param>
        /// <param name="nodeId">id of the new node</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>new node instance</returns>
        Task<INode> CreateNode(Guid nodeDescriptionGuid, Guid nodeId, CancellationToken cancellationToken);
    }
}
