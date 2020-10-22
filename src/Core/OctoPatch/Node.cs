using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctoPatch.ContentTypes;
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public NodeState State
        {
            get => _state;
            private set
            {
                _state = value;
                StateChanged?.Invoke(this, value);
            }
        }

        /// <inheritdoc />
        public IEnumerable<IInputConnector> Inputs => _inputs;

        /// <inheritdoc />
        public IEnumerable<IOutputConnector> Outputs => _outputs;

        /// <inheritdoc />
        public string GetEnvironment()
        {
            return JsonConvert.SerializeObject(Environment);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public Task Initialize(CancellationToken cancellationToken)
        {
            var configuration = JsonConvert.SerializeObject(DefaultConfiguration);
            return Initialize(configuration, cancellationToken);
        }

        /// <summary>
        /// Returns a default configuration for this node
        /// </summary>
        protected abstract TConfiguration DefaultConfiguration { get; }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// Gets a call on dispose
        /// </summary>
        protected virtual void OnDispose() { }

        #endregion

        #region Output Management

        /// <summary>
        /// Registers a new trigger output connector to the node
        /// </summary>
        /// <param name="description">output connector description</param>
        /// <returns>new connector</returns>
        protected IOutput RegisterOutput(ConnectorDescription description)
        {
            var outputConnector = OutputConnector.Create(Id, description);
            _outputs.Add(outputConnector);
            return new TriggerOutput(outputConnector);
        }

        /// <summary>
        /// Registers a new raw output connector to the node
        /// </summary>
        /// <param name="description">output connector description</param>
        /// <returns>new connector</returns>
        protected IRawOutput RegisterRawOutput(ConnectorDescription description)
        {
            var outputConnector = OutputConnector.Create(Id, description);
            _outputs.Add(outputConnector);
            return new Output(outputConnector);
        }

        /// <summary>
        /// Registers a new output connector to the node
        /// </summary>
        /// <param name="description">output connector description</param>
        /// <returns>new connector</returns>
        protected IOutput<T> RegisterOutput<T>(ConnectorDescription description) where T : struct
        {
            var outputConnector = OutputConnector.Create<T>(Id, description);
            _outputs.Add(outputConnector);
            return new StructOutput<T>(outputConnector);
        }

        /// <summary>
        /// Registers a new string output connector to the node
        /// </summary>
        /// <param name="description">output connector description</param>
        /// <returns>new connector</returns>
        protected IOutput<string> RegisterStringOutput(ConnectorDescription description)
        {
            var outputConnector = OutputConnector.Create<string>(Id, description);
            _outputs.Add(outputConnector);
            return new StringOutput(outputConnector);
        }

        /// <summary>
        /// Registers a new binary output connector to the node
        /// </summary>
        /// <param name="description">output connector description</param>
        /// <returns>new connector</returns>
        protected IOutput<byte[]> RegisterBinaryOutput(ConnectorDescription description)
        {
            var outputConnector = OutputConnector.Create<string>(Id, description);
            _outputs.Add(outputConnector);
            return new BinaryOutput(outputConnector);
        }

        /// <summary>
        /// Interface for basic raw output
        /// </summary>
        protected interface IRawOutput
        {
            /// <summary>
            /// Sends out raw message through the output
            /// </summary>
            /// <param name="message">raw message</param>
            void SendRaw(Message message);
        }

        /// <summary>
        /// Interface for a trigger output
        /// </summary>
        protected interface IOutput : IRawOutput
        {
            /// <summary>
            /// Sends a trigger through the output
            /// </summary>
            void Send();
        }

        /// <summary>
        /// Interface for value outputs
        /// </summary>
        /// <typeparam name="T">message type</typeparam>
        protected interface IOutput<in T> : IRawOutput
        {
            /// <summary>
            /// Sends the given message through the output
            /// </summary>
            /// <param name="input">message</param>
            void Send(T input);
        }

        /// <summary>
        /// Basic implementation for all outputs
        /// </summary>
        private class Output : IRawOutput
        {
            /// <summary>
            /// Reference to the connector
            /// </summary>
            private readonly OutputConnector _connector;

            public Output(OutputConnector connector)
            {
                _connector = connector ?? throw new ArgumentNullException(nameof(connector));
            }

            /// <inheritdoc />
            public void SendRaw(Message message)
            {
                _connector.Send(message);
            }
        }

        /// <summary>
        /// Handler for trigger outputs
        /// </summary>
        private sealed class TriggerOutput : Output, IOutput
        {
            public TriggerOutput(OutputConnector connector) : base(connector)
            {
            }

            /// <inheritdoc />
            public void Send()
            {
                SendRaw(Message.Create());
            }
        }

        /// <summary>
        /// Handler for value outputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class StructOutput<T> : Output, IOutput<T> where T : struct
        {
            public StructOutput(OutputConnector connector) : base(connector)
            {
            }

            /// <inheritdoc />
            public void Send(T input)
            {
                SendRaw(Message.Create(input));
            }
        }

        /// <summary>
        /// Handler for string outputs
        /// </summary>
        private sealed class StringOutput : Output, IOutput<string>
        {
            public StringOutput(OutputConnector connector) : base(connector)
            {
            }

            /// <inheritdoc />
            public void Send(string input)
            {
                var message = new Message(typeof(string),
                    new StringContentType.StringContainer { Content = input });
                SendRaw(message);
            }
        }

        /// <summary>
        /// Handler for binary outputs
        /// </summary>
        private sealed class BinaryOutput : Output, IOutput<byte[]>
        {
            public BinaryOutput(OutputConnector connector) : base(connector)
            {
            }

            /// <inheritdoc />
            public void Send(byte[] input)
            {
                // TODO: Think about cloning the array since this is a mutable type

                var message = new Message(typeof(byte[]), 
                    new BinaryContentType.BinaryContainer { Content = input });
                SendRaw(message);
            }
        }

        #endregion

        #region Input Management

        /// <summary>
        /// Registers a new raw input connector to the node
        /// </summary>
        /// <param name="description">input connector description</param>
        /// <param name="handler">message handler</param>
        /// <returns>new connector</returns>
        protected void RegisterRawInput(ConnectorDescription description, Action<Message> handler)
        {
            var inputConnector = InputConnector.Create(Id, description);
            _inputs.Add(inputConnector);

            if (handler != null)
            {
                inputConnector.Handle(handler.Invoke);
            }
        }

        /// <summary>
        /// Registers a new trigger input connector to the node
        /// </summary>
        /// <param name="description">input connector description</param>
        /// <param name="handler">message handler</param>
        /// <returns>new connector</returns>
        protected void RegisterTriggerInput(ConnectorDescription description, Action handler)
        {
            if (!description.ContentType.IsSupportedType(typeof(void)))
            {
                throw new NotSupportedException("handler does not fit to the connector description");
            }

            var inputConnector = InputConnector.Create(Id, description);
            _inputs.Add(inputConnector);

            if (handler != null)
            {
                inputConnector.Handle(m =>
                {
                    // Make sure message fits to the expected input
                    if (m.Type != typeof(void))
                    {
                        return;
                    }

                    handler.Invoke();
                });
            }
        }

        /// <summary>
        /// Registers a new input connector to the node
        /// </summary>
        /// <typeparam name="T">message type</typeparam>
        /// <param name="description">input connector description</param>
        /// <param name="handler">message handler</param>
        /// <returns>new connector</returns>
        protected void RegisterInput<T>(ConnectorDescription description, Action<T> handler) where T : struct
        {
            if (!description.ContentType.IsSupportedType<T>())
            {
                throw new NotSupportedException("handler does not fit to the connector description");
            }

            var inputConnector = InputConnector.Create<T>(Id, description);
            _inputs.Add(inputConnector);

            if (handler != null)
            {
                inputConnector.Handle(m =>
                {
                    // Make sure message fits to the expected input
                    if (m.Type != typeof(T))
                    {
                        return;
                    }

                    handler.Invoke((T)m.Content);
                });
            }
        }

        /// <summary>
        /// Registers a new string input connector to the node
        /// </summary>
        /// <param name="description">input connector description</param>
        /// <param name="handler">message handler</param>
        /// <returns>new connector</returns>
        protected void RegisterStringInput(ConnectorDescription description, Action<string> handler)
        {
            if (!description.ContentType.IsSupportedType<string>())
            {
                throw new NotSupportedException("handler does not fit to the connector description");
            }

            var inputConnector = InputConnector.Create(Id, description);
            _inputs.Add(inputConnector);

            if (handler != null)
            {
                inputConnector.Handle(m =>
                {
                    // Make sure message fits to the expected input
                    if (m.Type != typeof(string))
                    {
                        return;
                    }

                    handler.Invoke(((StringContentType.StringContainer)m.Content).Content);
                });
            }
        }

        /// <summary>
        /// Registers a new binary input connector to the node
        /// </summary>
        /// <param name="description">input connector description</param>
        /// <param name="handler">message handler</param>
        /// <returns>new connector</returns>
        protected void RegisterBinaryInput(ConnectorDescription description, Action<byte[]> handler)
        {
            if (!description.ContentType.IsSupportedType<byte[]>())
            {
                throw new NotSupportedException("handler does not fit to the connector description");
            }

            var inputConnector = InputConnector.Create(Id, description);
            _inputs.Add(inputConnector);

            if (handler != null)
            {
                inputConnector.Handle(m =>
                {
                    // Make sure message fits to the expected input
                    if (m.Type != typeof(byte[]))
                    {
                        return;
                    }

                    handler.Invoke(((BinaryContentType.BinaryContainer)m.Content).Content);
                });
            }
        }

        #endregion

        /// <inheritdoc />
        public event Action<INode, NodeState> StateChanged;

        /// <inheritdoc />
        public event Action<INode, string> ConfigurationChanged;

        /// <inheritdoc />
        public event Action<INode, string> EnvironmentChanged;
    }
}
