using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Represents the interface of a node within the OctoPatch grid
    /// StreamNote: TheBlubb14 was here (2020-06-30 20:47)
    /// </summary>
    public interface INode : IDisposable
    {
        /// <summary>
        /// Gets the node id. This is only set when initialized.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Returns the current state of the node
        /// </summary>
        NodeState State { get; }

        /// <summary>
        /// Gets the list of all inputs
        /// </summary>
        IEnumerable<IInputConnector> Inputs { get; }
        
        /// <summary>
        /// Gets the list of all outputs
        /// </summary>
        IEnumerable<IOutputConnector> Outputs { get; }

        /// <summary>
        /// Returns the current environment serialized as string
        /// </summary>
        /// <returns>serialized environment</returns>
        string GetEnvironment();

        /// <summary>
        /// Returns the current configuration serialized as string
        /// </summary>
        /// <returns>serialized configuration</returns>
        string GetConfiguration();

        /// <summary>
        /// Initializes the current node with the default configuration. This leads to a state
        /// change between <see cref="NodeState.Uninitialized"/> to <see cref="NodeState.Stopped"/>
        /// and can lead to <see cref="NodeState.InitializationFailed"/> in case of an error.
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Initialize(CancellationToken cancellationToken);

        /// <summary>
        /// Initializes the current node. This leads to a state change between <see cref="NodeState.Uninitialized"/>
        /// to <see cref="NodeState.Stopped"/> and can lead to <see cref="NodeState.InitializationFailed"/> in case of an error.
        /// </summary>
        /// <param name="configuration">configuration string</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task Initialize(string configuration, CancellationToken cancellationToken);

        /// <summary>
        /// Starts the node and set state to <see cref="NodeState.Running"/> and can lead to <see cref="NodeState.Failed"/> in case of an error.
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Stops the node and set state to <see cref="NodeState.Stopped"/> and can lead to <see cref="NodeState.Failed"/> in case of an error.
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Stop(CancellationToken cancellationToken);

        /// <summary>
        /// Deinitializes the node. This leads to <see cref="NodeState.Uninitialized"/>
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Deinitialize(CancellationToken cancellationToken);

        /// <summary>
        /// Gets a call when the current node state changes.
        /// </summary>
        event Action<INode, NodeState> StateChanged;

        /// <summary>
        /// Gets a call when the current configuration changes.
        /// </summary>
        event Action<INode, string> ConfigurationChanged;

        /// <summary>
        /// Gets a call when the current environment changes.
        /// </summary>
        event Action<INode, string> EnvironmentChanged;
    }
}
