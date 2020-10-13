using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Default adapter to pass data from input to output. This will be used in case of no other adapter is configured.
    /// </summary>
    public sealed class PassAdapter : IAdapter
    {
        /// <summary>
        /// Holds the local subscription
        /// </summary>
        private readonly IDisposable _subscription;

        /// <inheritdoc />
        public IWire Wire { get; }

        public PassAdapter(IWire wire)
        {
            Wire = wire ?? throw new ArgumentNullException(nameof(wire));

            // Just pass data
            _subscription = wire.Input.Subscribe(wire.Output);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _subscription.Dispose();
        }


        /// <inheritdoc />
        public string GetEnvironment()
        {
            // Pass adapter has no environment
            return null;
        }

        /// <inheritdoc />
        public string GetConfiguration()
        {
            // Pass adapter has no configuration
            return null;
        }

        /// <inheritdoc />
        public Task SetConfiguration(string configuration, CancellationToken cancellationToken)
        {
            // Pass adpater has no configuration so do nothing here
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public event Action<IAdapter, string> ConfigurationChanged;

        /// <inheritdoc />
        public event Action<IAdapter, string> EnvironmentChanged;
    }
}
