using OctoPatch.Communication;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Represents the interface of a node within the OctoPatch grid
    /// StreamNote: TheBlubb14 was here (2020-06-30 20:47)
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Returns the current state of the node
        /// </summary>
        NodeState State { get; }

        /// <summary>
        /// Gets the current node instance information
        /// </summary>
        NodeInstance Instance { get; }

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
        /// Configures the current node instance
        /// </summary>
        /// <param name="instance">instance information</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task Setup(NodeInstance instance, CancellationToken cancellationToken);

        /// <summary>
        /// Starts the node and set state to <see cref="NodeState.Running"/>
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Stops the node and set state to <see cref="NodeState.Stopped"/>
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Stop(CancellationToken cancellationToken);

        /// <summary>
        /// Disposes the node and reset it to <see cref="NodeState.NotReady"/>
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Dispose(CancellationToken cancellationToken);

        /// <summary>
        /// Resets the node from <see cref="NodeState.Failed"/> to <see cref="NodeState.NotReady"/>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Reset(CancellationToken cancellationToken);

        #endregion
    }
}
