using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch.Core
{
    /// <summary>
    /// Represents the interface of a node within the OctoPatch grid
    /// StreamNote: TheBlubb14 was here (2020-06-30 20:47)
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Gets the node id. This is only set when initialized.
        /// </summary>
        Guid NodeId { get; }

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

        #region Lifecycle methods

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

        #endregion
    }
}
