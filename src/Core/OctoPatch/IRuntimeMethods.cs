using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;
using OctoPatch.Setup;

namespace OctoPatch
{
    /// <summary>
    /// interface with all the methods of the patch runtime
    /// </summary>
    public interface IRuntimeMethods
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
        Task<IEnumerable<TypeDescription>> GetMessageDescriptions(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the fill list of known adapter descriptions
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>all adapter descriptions</returns>
        Task<IEnumerable<AdapterDescription>> GetAdapterDescriptions(CancellationToken cancellationToken);

        #endregion

        #region Patch manipulation

        /// <summary>
        /// Returns a list of nodes
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>list of nodes</returns>
        Task<IEnumerable<NodeSetup>> GetNodes(CancellationToken cancellationToken);

        /// <summary>
        /// Returns a list of wires
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>list of wires</returns>
        Task<IEnumerable<WireSetup>> GetWires(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the whole configuration
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>configuration</returns>
        Task<GridSetup> GetConfiguration(CancellationToken cancellationToken);

        /// <summary>
        /// Applies the given configuration
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="grid">configuration</param>
        Task SetConfiguration(GridSetup grid, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new common node to the patch
        /// </summary>
        /// <param name="key">unique key for the node description</param>
        /// <param name="connectorKey">optional connector key. Is used for splitters and collectors only</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="parentId">optional parent id. Is used for attached nodes, splitter or collectors</param>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <returns>new node setup</returns>
        Task<NodeSetup> AddNode(string key, Guid? parentId, string connectorKey, int x, int y, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the node with the given id from the patch
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveNode(Guid nodeId, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new wire to the patch
        /// </summary>
        /// <param name="outputNodeId">node id of the output node</param>
        /// <param name="outputConnectorKey">connector key for the output</param>
        /// <param name="inputNodeId">node id of the input node</param>
        /// <param name="inputConnectorKey">connector key for the input</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>new wire instance</returns>
        Task<WireSetup> AddWire(Guid outputNodeId, string outputConnectorKey, Guid inputNodeId, string inputConnectorKey,
            CancellationToken cancellationToken);

        /// <summary>
        /// Removes the wire with the given id
        /// </summary>
        /// <param name="wireId">wire id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveWire(Guid wireId, CancellationToken cancellationToken);

        #endregion

        #region Node configuration

        /// <summary>
        /// Sets the name and the description of a single node instance
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="name">new name for this node</param>
        /// <param name="description">optional description for the block</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task SetNodeDescription(Guid nodeId, string name, string description, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the new position of the given node
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns></returns>
        Task SetNodePosition(Guid nodeId, int x, int y, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the current environment information of the requested node
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>serialized environment information</returns>
        Task<string> GetNodeEnvironment(Guid nodeId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the configuration from the given node
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>serialized configuration for this node</returns>
        Task<string> GetNodeConfiguration(Guid nodeId, CancellationToken cancellationToken);

        /// <summary>
        /// Applies the given serialized configuration to the node
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="configuration">configuration</param>
        Task SetNodeConfiguration(Guid nodeId, string configuration, CancellationToken cancellationToken);

        /// <summary>
        /// Starts the given node
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task StartNode(Guid nodeId, CancellationToken cancellationToken);

        /// <summary>
        /// Stops the given node
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task StopNode(Guid nodeId, CancellationToken cancellationToken);

        #endregion

        #region Wire / Adapter configuration

        /// <summary>
        /// Sets the current adapter for a specific wire
        /// </summary>
        /// <param name="wireId">target wire</param>
        /// <param name="key">adapter key</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task SetAdapter(Guid wireId, string key, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the current adapter environment for the given wire
        /// </summary>
        /// <param name="wireId">target wire</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>serialized configuration</returns>
        Task<string> GetAdapterEnvironment(Guid wireId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the current configuration of the adapter of the given wire
        /// </summary>
        /// <param name="wireId">target wire</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>serialized configuration</returns>
        Task<string> GetAdapterConfiguration(Guid wireId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the given configuration at the target adapter
        /// </summary>
        /// <param name="wireId">target wire</param>
        /// <param name="configuration">serialized configuration</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task SetAdapterConfiguration(Guid wireId, string configuration, CancellationToken cancellationToken);

        #endregion
    }
}
