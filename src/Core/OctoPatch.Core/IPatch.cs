using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch.Core
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
        /// Removes the given node and deletes also all related wires.
        /// </summary>
        /// <param name="node">node reference</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveNode(INode node, CancellationToken cancellationToken);

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
        /// <param name="outputConnector">connector id of the output</param>
        /// <param name="inputNode">node id of the input node</param>
        /// <param name="inputConnector">connector id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>reference to the new wire</returns>
        Task<IWire> AddWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the wire with the given parameters
        /// </summary>
        /// <param name="outputNode">node id of the output node</param>
        /// <param name="outputConnector">connector id of the output</param>
        /// <param name="inputNode">node id of the input node</param>
        /// <param name="inputConnector">connector id</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the given wire
        /// </summary>
        /// <param name="wire">reference to the wire</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task RemoveWire(IWire wire, CancellationToken cancellationToken);
    }
}
