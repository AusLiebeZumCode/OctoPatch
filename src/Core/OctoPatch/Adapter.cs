using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of adapters
    /// </summary>
    /// <typeparam name="TConfiguration">type of configuration</typeparam>
    /// <typeparam name="TEnvironment">type of environment</typeparam>
    public abstract class Adapter<TConfiguration, TEnvironment> : IAdapter
        where TConfiguration : IConfiguration
        where TEnvironment : IEnvironment
    {
        /// <summary>
        /// Reference to the current subscription
        /// </summary>
        private readonly IDisposable _subscription;

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

        /// <summary>
        /// Internal reference to the input connector
        /// </summary>
        protected IOutputConnector Input { get; }

        /// <summary>
        /// Internal reference to the output connector
        /// </summary>
        protected IInputConnector Output { get; }

        /// <inheritdoc />
        public IWire Wire { get; }

        protected Adapter(IWire wire)
        {
            Wire = wire ?? throw new ArgumentNullException(nameof(wire));
            Input = wire.Input ?? throw new ArgumentNullException(nameof(wire.Input));
            Output = wire.Output ?? throw new ArgumentNullException(nameof(wire.Output));

            _subscription = Wire.Input.Subscribe(message =>
            {
                Wire.Output.OnNext(Handle(message));
            });
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        protected abstract Message Handle(Message message);

        /// <summary>
        /// Sets a new configuration to the adapter
        /// </summary>
        /// <param name="configuration">serialized configuration</param>
        /// <param name="cancellationToken">cancellation token</param>
        public async Task SetConfiguration(string configuration, CancellationToken cancellationToken)
        {
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
            await OnSetConfiguration(config, cancellationToken);

            // Write back values
            Configuration = config;
        }

        /// <summary>
        /// Gets a call when node is in setup
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="cancellationToken">cancellation token</param>
        protected virtual Task OnSetConfiguration(TConfiguration configuration, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

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

        /// <inheritdoc />
        public event Action<IAdapter, string> ConfigurationChanged;
        
        /// <inheritdoc />
        public event Action<IAdapter, string> EnvironmentChanged;
    }
}
