using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Interface for the central patch
    /// </summary>
    public interface IPatch
    {
        /// <summary>
        /// Adds a new node
        /// </summary>
        /// <param name="node">node instance</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>new node</returns>
        Task AddNode(INode node, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new node and configures it
        /// </summary>
        /// <param name="node">node instance</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="configuration">configuration string</param>
        /// <returns>new node</returns>
        Task AddNode(INode node, string configuration, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the node with the given id and deletes also all related wires.
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveNode(Guid nodeId, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a wire between the given nodes and outputs.
        /// </summary>
        /// <param name="outputNode">node id of the output node</param>
        /// <param name="outputConnector">connector key of the output</param>
        /// <param name="inputNode">node id of the input node</param>
        /// <param name="inputConnector">connector key</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>reference to the new wire</returns>
        Task<IWire> AddWire(Guid outputNode, string outputConnector, Guid inputNode, string inputConnector, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the given wire
        /// </summary>
        /// <param name="wireId">wire id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveWire(Guid wireId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a call when a new node was added
        /// </summary>
        event Action<INode> NodeAdded;

        /// <summary>
        /// Gets a call when a node was removed
        /// </summary>
        event Action<INode> NodeRemoved;

        /// <summary>
        /// Gets a call when a new wire was added
        /// </summary>
        event Action<IWire> WireAdded;

        /// <summary>
        /// Gets a call when a wire was removed
        /// </summary>
        event Action<IWire> WireRemoved;
    }
}
