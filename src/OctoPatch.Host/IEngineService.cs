using System;
using System.Collections.Generic;
using OctoPatch.Exchange;

namespace OctoPatch.Host
{
    /// <summary>
    /// Communication interface between configuration client and the engine
    /// </summary>
    public interface IEngineService
    {
        #region Meta information

        /// <summary>
        /// Returns the full list of known node descriptions
        /// </summary>
        /// <returns>all node descriptions</returns>
        IEnumerable<NodeDescription> GetNodeDescriptions();

        /// <summary>
        /// Returns the full list of known message descriptions
        /// </summary>
        /// <returns>all message descriptions</returns>
        IEnumerable<MessageDescription> GetMessageDescriptions();

        #endregion

        #region Engine manipulation

        /// <summary>
        /// Returns a list of nodes within the current engine
        /// </summary>
        /// <returns>list of nodes</returns>
        IEnumerable<NodeInstance> GetNodes();

        /// <summary>
        /// Returns a list of wires within the current engine
        /// </summary>
        /// <returns>list of wires</returns>
        IEnumerable<WireInstance> GetWires();

        /// <summary>
        /// Returns the whole engine configuration
        /// </summary>
        /// <returns>configuration</returns>
        Grid GetEngineConfiguration();

        /// <summary>
        /// Applies the given configuration to the current engine
        /// </summary>
        /// <param name="grid">configuration</param>
        void SetEngineConfiguration(Grid grid);

        #endregion

        #region Node configuration

        /// <summary>
        /// Gets the current environment information of the requested node
        /// </summary>
        /// <param name="nodeGuid">guid of node</param>
        /// <returns>serialized environment information</returns>
        string GetNodeEnvironment(Guid nodeGuid);

        /// <summary>
        /// Gets the configuration from the given node
        /// </summary>
        /// <param name="nodeGuid">node guid</param>
        /// <returns>serialized configuration for this node</returns>
        string GetNodeConfiguration(Guid nodeGuid);

        /// <summary>
        /// Applies the given serialized configuration to the node
        /// </summary>
        /// <param name="nodeGuid">node guid</param>
        /// <param name="configuration">configuration</param>
        void SetNodeConfiguration(Guid nodeGuid, string configuration);

        #endregion
    }
}
