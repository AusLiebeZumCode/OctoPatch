using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of node implementations
    /// </summary>
    /// <typeparam name="TConfiguration">configuration type</typeparam>
    /// <typeparam name="TEnvironment">environment type</typeparam>
    public abstract class Node<TConfiguration, TEnvironment> : INode
        where TConfiguration : IConfiguration
        where TEnvironment : IEnvironment
    {
        /// <summary>
        /// Local lock to synchronize the node lifecycle
        /// </summary>
        private readonly SemaphoreSlim _localLock;

        private readonly List<IInputConnector> _inputs;

        private readonly List<IOutputConnector> _outputs;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Guid Id { get; }

        private TConfiguration _configuration;

        /// <summary>
        /// Internal reference to the current configuration
        /// </summary>
        protected TConfiguration Configuration
        {
            get => _configuration;
            private set
            {
                _configuration = value;
                ConfigurationChanged?.Invoke(this, GetEnvironment());
            }
        }

        private TEnvironment _environment;

        /// <summary>
        /// Internal reference to the current environment
        /// </summary>
        protected TEnvironment Environment
        {
            get => _environment;
            private set
            {
                _environment = value;
                EnvironmentChanged?.Invoke(this, GetEnvironment());
            }
        }

        private NodeState _state;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public NodeState State
        {
            get => _state;
            private set
            {
                _state = value;
                StateChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<IInputConnector> Inputs => _inputs;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<IOutputConnector> Outputs => _outputs;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string GetEnvironment()
        {
            return JsonConvert.SerializeObject(Environment);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string GetConfiguration()
        {
            return JsonConvert.SerializeObject(Configuration);
        }

        protected Node(Guid id)
        {
            _localLock = new SemaphoreSlim(1);

            Id = id;
            State = NodeState.Uninitialized;

            _inputs = new List<IInputConnector>();
            _outputs = new List<IOutputConnector>();
        }

        protected void UpdateEnvironment(TEnvironment environment)
        {
            Environment = environment;
        }

        #region Lifecycle methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task Initialize(CancellationToken cancellationToken)
        {
            var configuration = JsonConvert.SerializeObject(DefaultConfiguration);
            return Initialize(configuration, cancellationToken);
        }

        /// <summary>
        /// Returns a default configuration for this node
        /// </summary>
        protected abstract TConfiguration DefaultConfiguration { get; }

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
                TConfiguration config;
                try
                {
                    config = JsonConvert.DeserializeObject<TConfiguration>(configuration);
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
        protected virtual Task OnInitialize(TConfiguration configuration, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

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
        protected virtual Task OnStart(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

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
        protected virtual Task OnStop(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

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
        protected virtual Task OnDeinitialize(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

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
        protected virtual Task OnInitializeReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

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
        protected virtual Task OnReset(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

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

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// Gets a call on dispose
        /// </summary>
        protected virtual void OnDispose() { }

        #endregion

        #region Connector Management

        /// <summary>
        /// Registers a new output connector to the node
        /// </summary>
        /// <param name="description">output connector description</param>
        /// <returns>new connector</returns>
        protected IOutputConnectorHandler RegisterOutputConnector(ConnectorDescription description)
        {
            var outputConnector = new OutputConnector(Id, description);
            _outputs.Add(outputConnector);
            return outputConnector;
        }

        /// <summary>
        /// Registers a new input connector to the node
        /// </summary>
        /// <param name="description">input connector description</param>
        /// <returns>new connector</returns>
        protected IInputConnectorHandler RegisterInputConnector(ConnectorDescription description)
        {
            var inputConnector = new InputConnector(Id, description);
            _inputs.Add(inputConnector);
            return inputConnector;
        }

        #endregion

        /// <summary>
        /// Gets a call when the current node state changes.
        /// </summary>
        public event Action<INode, NodeState> StateChanged;

        /// <summary>
        /// Gets a call when the current configuration changes.
        /// </summary>
        public event Action<INode, string> ConfigurationChanged;

        /// <summary>
        /// Gets a call when the current environment changes.
        /// </summary>
        public event Action<INode, string> EnvironmentChanged;
    }
}
