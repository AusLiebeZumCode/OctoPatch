using System;
using System.Collections.Generic;
using OctoPatch.Exchange;

namespace OctoPatch
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
        /// <param name="nodeGuid">guid of node to generate an instance of</param>
        /// <returns>new node instance</returns>
        INode CreateNode(Guid nodeGuid);
    }
}
