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

        protected Adapter(IOutputConnector input, IInputConnector output)
        {
        }

        public void Dispose()
        {

        }

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
            await OnInitialize(config, cancellationToken);

            // Write back values
            Configuration = config;
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

        /// <inheritdoc />
        public event Action<IAdapter, string> ConfigurationChanged;
        
        /// <inheritdoc />
        public event Action<IAdapter, string> EnvironmentChanged;
    }
}
