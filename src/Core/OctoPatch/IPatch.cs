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
        /// Removes the node with the given id and deletes also all related wires.
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveNode(Guid nodeId, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a wire between the given nodes and outputs.
        /// </summary>
        /// <param name="wire">wire instance</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task AddWire(IWire wire, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the given wire
        /// </summary>
        /// <param name="wireId">wire id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveWire(Guid wireId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets a new adapter for the given node
        /// </summary>
        /// <param name="wireId">wire id</param>
        /// <param name="adapter">adapter instance</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task AddAdapter(Guid wireId, IAdapter adapter, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the adapter of the given wire
        /// </summary>
        /// <param name="wireId">wire id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveAdapter(Guid wireId, CancellationToken cancellationToken);

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

        /// <summary>
        /// Gets a call when a new adapter was added
        /// </summary>
        event Action<IWire, IAdapter> AdapterAdded;

        /// <summary>
        /// Gets a call when an adapter was removed
        /// </summary>
        event Action<IWire, IAdapter> AdapterRemoved;
    }
}
