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
    }
}
