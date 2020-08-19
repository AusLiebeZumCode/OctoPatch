using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
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
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>all node descriptions</returns>
        Task<IEnumerable<NodeDescription>> GetNodeDescriptions(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the full list of known message descriptions
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>all message descriptions</returns>
        Task<IEnumerable<MessageDescription>> GetMessageDescriptions(CancellationToken cancellationToken);

        #endregion

        #region Engine manipulation

        /// <summary>
        /// Returns a list of nodes within the current engine
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>list of nodes</returns>
        Task<IEnumerable<NodeInstance>> GetNodes(CancellationToken cancellationToken);

        /// <summary>
        /// Returns a list of wires within the current engine
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>list of wires</returns>
        Task<IEnumerable<WireInstance>> GetWires(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the whole engine configuration
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>configuration</returns>
        Task<Grid> GetEngineConfiguration(CancellationToken cancellationToken);

        /// <summary>
        /// Applies the given configuration to the current engine
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="grid">configuration</param>
        Task SetEngineConfiguration(Grid grid, CancellationToken cancellationToken);

        Task<NodeInstance> AddNode(Guid nodeDescriptionGuid, CancellationToken cancellationToken);

        Task RemoveNode(Guid nodeId, CancellationToken cancellationToken);

        Task<WireInstance> AddWire(Guid outputNodeId, Guid outputConnectorId, Guid inputNodeId, Guid intputConnectorId,
            CancellationToken cancellationToken);

        Task RemoveWire(Guid wireId, CancellationToken cancellationToken);

        #endregion

        #region Node configuration

        /// <summary>
        /// Gets the current environment information of the requested node
        /// </summary>
        /// <param name="nodeGuid">guid of node</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>serialized environment information</returns>
        Task<string> GetNodeEnvironment(Guid nodeGuid, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the configuration from the given node
        /// </summary>
        /// <param name="nodeGuid">node guid</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>serialized configuration for this node</returns>
        Task<string> GetNodeConfiguration(Guid nodeGuid, CancellationToken cancellationToken);

        /// <summary>
        /// Applies the given serialized configuration to the node
        /// </summary>
        /// <param name="nodeGuid">node guid</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="configuration">configuration</param>
        Task SetNodeConfiguration(Guid nodeGuid, string configuration, CancellationToken cancellationToken);

        #endregion
    }
}
