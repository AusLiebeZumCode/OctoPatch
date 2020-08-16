using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Communication;

namespace OctoPatch
{
    /// <summary>
    /// Interface for the central grid runtime
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// List of all node instances
        /// </summary>
        ReadOnlyObservableCollection<INode> Nodes { get; }

        /// <summary>
        /// List of all wires
        /// </summary>
        ReadOnlyObservableCollection<IWire> Wires { get; }

        /// <summary>
        /// Loads the given grid into the engine
        /// </summary>
        /// <param name="grid">grid setup</param>
        void Load(Grid grid);

        /// <summary>
        /// Returns the current grid setup
        /// </summary>
        /// <returns>current grid setup</returns>
        Grid Store();

        #region engine lifecycle

        /// <summary>
        /// Gets the current engine state
        /// </summary>
        EngineState State { get; }

        /// <summary>
        /// Starts the engine
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Stops the engine
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Stop(CancellationToken cancellationToken);

        #endregion

        #region node management

        /// <summary>
        /// Adds a new node to the engine
        /// </summary>
        /// <param name="type">type of node</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>new node</returns>
        Task<INode> AddNode(Type type, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new node to the engine and configures it
        /// </summary>
        /// <param name="type">type of node</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="configuration">configuration string</param>
        /// <returns>new node</returns>
        Task<INode> AddNode(Type type, string configuration, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new node to the engine and configures it
        /// </summary>
        /// <param name="type">type of node</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="nodeId">id of the new node</param>
        /// <param name="configuration">configuration string</param>
        /// <returns>new node</returns>
        Task<INode> AddNode(Type type, Guid nodeId, string configuration, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new node to the engine
        /// </summary>
        /// <param name="descriptionId">id of the node description</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>new node</returns>
        Task<INode> AddNode(Guid descriptionId, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new node to the engine and configures it
        /// </summary>
        /// <param name="descriptionId">id of the node description</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="configuration">configuration string</param>
        /// <returns>new node</returns>
        Task<INode> AddNode(Guid descriptionId, string configuration, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new node to the engine and configures it
        /// </summary>
        /// <param name="descriptionId">id of the node description</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="nodeId">id of the new node</param>
        /// <param name="configuration">configuration string</param>
        /// <returns>new node</returns>
        Task<INode> AddNode(Guid descriptionId, Guid nodeId, string configuration, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new node to the engine
        /// </summary>
        /// <typeparam name="T">type of node</typeparam>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>new node</returns>
        Task<INode> AddNode<T>(CancellationToken cancellationToken) where T : INode;

        /// <summary>
        /// Adds a new node to the engine and configures it
        /// </summary>
        /// <typeparam name="T">type of node</typeparam>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="configuration">configuration string</param>
        /// <returns>new node</returns>
        Task<INode> AddNode<T>(string configuration, CancellationToken cancellationToken) where T : INode;

        /// <summary>
        /// Adds a new node to the engine and configures it
        /// </summary>
        /// <typeparam name="T">type of node</typeparam>
        /// <param name="cancellationToken">cancellation token</param>
        /// <param name="nodeId">id of the new node</param>
        /// <param name="configuration">configuration string</param>
        /// <returns>new node</returns>
        Task<INode> AddNode<T>(Guid nodeId, string configuration, CancellationToken cancellationToken) where T : INode;

        /// <summary>
        /// Removes the given node from the engine and deletes also all related wires.
        /// </summary>
        /// <param name="node">node reference</param>
        Task RemoveNode(INode node);

        /// <summary>
        /// Removes the node with the given id and deletes also all related wires.
        /// </summary>
        /// <param name="nodeId">node id</param>
        Task RemoveNode(Guid nodeId);

        #endregion

        #region wire management

        /// <summary>
        /// Adds a wire between the given nodes and outputs.
        /// </summary>
        /// <param name="outputNode">node id of the output node</param>
        /// <param name="outputConnector">connector id of the output</param>
        /// <param name="inputNode">node id of the input node</param>
        /// <param name="inputConnector">connector id</param>
        /// <returns>reference to the new wire</returns>
        Task<IWire> AddWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector);

        /// <summary>
        /// Removes the wire with the given parameters
        /// </summary>
        /// <param name="outputNode">node id of the output node</param>
        /// <param name="outputConnector">connector id of the output</param>
        /// <param name="inputNode">node id of the input node</param>
        /// <param name="inputConnector">connector id</param>
        Task RemoveWire(Guid outputNode, Guid outputConnector, Guid inputNode, Guid inputConnector);

        /// <summary>
        /// Removes the given wire
        /// </summary>
        /// <param name="wire">reference to the wire</param>
        Task RemoveWire(IWire wire);

        #endregion
    }
}
