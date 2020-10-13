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
        /// <exception cref="ArgumentNullException">In case node is null</exception>
        /// <exception cref="ArgumentException">In case something is wrong with the node</exception>
        Task AddNode(INode node, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the node with the given id and deletes also all related wires.
        /// </summary>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>success flag. true in case of a properly removed item. false, when the node could not be found</returns>
        /// <exception cref="ArgumentException">In case the node is still referenced</exception>
        Task<bool> RemoveNode(Guid nodeId, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a wire between the given nodes and outputs.
        /// </summary>
        /// <param name="wire">wire instance</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <exception cref="ArgumentNullException">Missing parameter</exception>
        /// <exception cref="ArgumentException">In case input is invalid (missing references)</exception>
        Task AddWire(IWire wire, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the given wire
        /// </summary>
        /// <param name="wireId">wire id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>success</returns>
        Task<bool> RemoveWire(Guid wireId, CancellationToken cancellationToken);

        /// <summary>
        /// Sets a new adapter for the given node
        /// </summary>
        /// <param name="adapter">adapter instance</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <exception cref="ArgumentNullException">when adapter is incomplete</exception>
        /// <exception cref="ArgumentException">when adapter does not fit or input is invalid</exception>
        Task AddAdapter(IAdapter adapter, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the adapter of the given wire
        /// </summary>
        /// <param name="wireId">wire id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task<bool> RemoveAdapter(Guid wireId, CancellationToken cancellationToken);

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
