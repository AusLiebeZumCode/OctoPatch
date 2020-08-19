using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OctoPatch.Core
{
    /// <summary>
    /// Base class for all kind of node implementations
    /// </summary>
    /// <typeparam name="T">configuration type</typeparam>
    public abstract class Node<T> : INode where T : INodeConfiguration
    {
        /// <summary>
        /// Local lock to synchronize the node lifecycle
        /// </summary>
        private readonly SemaphoreSlim _localLock;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Guid NodeId { get; }

        /// <summary>
        /// Internal reference to the current configuration
        /// </summary>
        protected T Configuration { get; private set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public NodeState State { get; private set; }

        protected readonly List<IInputConnector> _inputs;

        protected readonly List<IOutputConnector> _outputs;


        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<IInputConnector> Inputs => _inputs;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<IOutputConnector> Outputs => _outputs;

        protected Node(Guid nodeId)
        {
            _localLock = new SemaphoreSlim(1);

            NodeId = nodeId;
            State = NodeState.Uninitialized;

            _inputs = new List<IInputConnector>();
            _outputs = new List<IOutputConnector>();
        }

        #region Lifecycle methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task Initialize(string configuration, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                switch (State)
                {
                    case NodeState.Uninitialized:
                        await InternalInitialize(configuration, cancellationToken);
                        break;
                    case NodeState.Stopped:
                        await InternalDeinitialize(cancellationToken);
                        await InternalInitialize(configuration, cancellationToken);
                        break;
                    case NodeState.Running:
                        await InternalStop(cancellationToken);
                        await InternalDeinitialize(cancellationToken);
                        await InternalInitialize(configuration, cancellationToken);
                        await InternalStart(cancellationToken);
                        break;
                    case NodeState.InitializationFailed:
                        await InternalInitializeReset(cancellationToken);
                        await InternalInitialize(configuration, cancellationToken);
                        break;
                    case NodeState.Failed:
                        await InternalReset(cancellationToken);
                        await InternalDeinitialize(cancellationToken);
                        await InternalInitialize(configuration, cancellationToken);
                        break;
                    default:
                        throw new NotSupportedException($"node is in the wrong state ({State})");
                }
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Internal initialization without lock. This method is used to chain transitions.
        /// </summary>
        /// <param name="configuration">configuration string</param>
        /// <param name="cancellationToken">cancellation token</param>
        private async Task InternalInitialize(string configuration, CancellationToken cancellationToken)
        {
            var previousState = State;
            try
            {
                State = NodeState.Initializing;

                if (string.IsNullOrEmpty(configuration))
                {
                    throw new ArgumentNullException(nameof(configuration));
                }

                // Deserialization of configuration
                T config;
                try
                {
                    config = JsonConvert.DeserializeObject<T>(configuration);
                }
                catch (JsonSerializationException ex)
                {
                    throw new ArgumentException("could not deserialize configuration", ex);
                }
                catch (JsonReaderException ex)
                {
                    throw new ArgumentException("could not read configuration", ex);
                }

                // Initialize
                await OnInitialize(config, cancellationToken);

                // Write back values
                Configuration = config;

                State = NodeState.Stopped;
            }
            catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                // Reset state back to previous state
                State = previousState;
                throw;
            }
            catch (Exception)
            {
                // set to fail state
                InternalFail(false);
                throw;
            }
        }

        /// <summary>
        /// Gets a call when node is in setup
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="cancellationToken">cancellation token</param>
        protected abstract Task OnInitialize(T configuration, CancellationToken cancellationToken);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task Start(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                switch (State)
                {
                    case NodeState.Stopped:
                        await InternalStart(cancellationToken);
                        break;
                    case NodeState.Running:
                        // Do nothing
                        break;
                    case NodeState.Failed:
                        await InternalReset(cancellationToken);
                        await InternalStart(cancellationToken);
                        break;
                    case NodeState.InitializationFailed:
                    case NodeState.Uninitialized:
                        throw new NotSupportedException($"node has no proper configuration to start");
                    default:
                        throw new NotSupportedException($"node is in the wrong state ({State})");
                }
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Internal start without lock. This method is used to chain transitions.
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        private async Task InternalStart(CancellationToken cancellationToken)
        {
            var previousState = State;
            try
            {
                State = NodeState.Starting;
                await OnStart(cancellationToken);
                State = NodeState.Running;
            }
            catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                // Reset state back to previous state
                State = previousState;
                throw;
            }
            catch (Exception)
            {
                // set to fail state
                InternalFail(true);
                throw;
            }
        }

        /// <summary>
        /// Gets a call when node is starting
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        protected abstract Task OnStart(CancellationToken cancellationToken);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task Stop(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                switch (State)
                {
                    case NodeState.Stopped:
                        // do nothing. Already in the right state
                        break;
                    case NodeState.Running:
                        await InternalStop(cancellationToken);
                        break;
                    case NodeState.Failed:
                        await InternalReset(cancellationToken);
                        break;
                    case NodeState.InitializationFailed:
                    case NodeState.Uninitialized:
                        throw new NotSupportedException($"node has no proper configuration to start");
                    default:
                        throw new NotSupportedException($"node is in the wrong state ({State})");
                }
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Internal stop without lock. This method is used to chain transitions.
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        private async Task InternalStop(CancellationToken cancellationToken)
        {
            var previousState = State;
            try
            {
                State = NodeState.Stopping;
                await OnStop(cancellationToken);
                State = NodeState.Stopped;
            }
            catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                // Reset state back to previous state
                State = previousState;
                throw;
            }
            catch (Exception)
            {
                // set to fail state
                InternalFail(true);
                throw;
            }
        }

        /// <summary>
        /// Gets a call when node is stopping
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        protected abstract Task OnStop(CancellationToken cancellationToken);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task Deinitialize(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                switch (State)
                {
                    case NodeState.Uninitialized:
                        // no action
                        break;
                    case NodeState.Stopped:
                        await InternalDeinitialize(cancellationToken);
                        break;
                    case NodeState.Running:
                        await InternalStop(cancellationToken);
                        await InternalDeinitialize(cancellationToken);
                        break;
                    case NodeState.InitializationFailed:
                        await InternalInitializeReset(cancellationToken);
                        break;
                    case NodeState.Failed:
                        await InternalReset(cancellationToken);
                        await InternalDeinitialize(cancellationToken);
                        break;
                    default:
                        throw new NotSupportedException($"node is in the wrong state ({State})");
                }
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Internal deinitialize call without lock. This method is used to chain transitions.
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        private async Task InternalDeinitialize(CancellationToken cancellationToken)
        {
            var previousState = State;
            try
            {
                State = NodeState.Deinitializing;
                await OnDeinitialize(cancellationToken);
            }
            catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                // Reset state back to previous state
                State = previousState;
                throw;
            }
            catch (Exception)
            {
                // All exceptions are ignored
                // TODO: but log this of course
            }

            State = NodeState.Uninitialized;
        }

        /// <summary>
        /// Gets a call when node gets disposed
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        protected abstract Task OnDeinitialize(CancellationToken cancellationToken);

        /// <summary>
        /// Internal initialize reset call without lock. This method is used to chain transitions.
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        private async Task InternalInitializeReset(CancellationToken cancellationToken)
        {
            var previousState = State;
            try
            {
                State = NodeState.Resetting;
                await OnInitializeReset(cancellationToken);
            }
            catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                // Reset state back to previous state
                State = previousState;
                throw;
            }
            catch (Exception)
            {
                // All exceptions are ignored
                // TODO: but log this of course
            }

            State = NodeState.Uninitialized;
        }

        /// <summary>
        /// Gets a call when node gets a reset for issues during initialization
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        protected abstract Task OnInitializeReset(CancellationToken cancellationToken);

        /// <summary>
        /// Internal reset call without lock. This method is used to chain transitions.
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        private async Task InternalReset(CancellationToken cancellationToken)
        {
            var previousState = State;
            try
            {
                State = NodeState.Resetting;
                await OnReset(cancellationToken);
            }
            catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
            {
                // Reset state back to previous state
                State = previousState;
                throw;
            }
            catch (Exception)
            {
                // All exceptions are ignored
                // TODO: but log this of course
            }

            State = NodeState.Stopped;
        }

        /// <summary>
        /// Gets a call when node gets a reset for issues during runtime
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        protected abstract Task OnReset(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the node state to failed based on an internal execution
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        protected async Task Fail(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                InternalFail(true);
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Internal fail call without lock. This method is used to chain transitions.
        /// </summary>
        /// <param name="initialized">true when failure happens in a initialized node</param>
        private void InternalFail(bool initialized)
        {
            State = initialized ? NodeState.Failed : NodeState.InitializationFailed;
        }

        #endregion
    }
}
