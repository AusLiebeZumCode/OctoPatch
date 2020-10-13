using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoPatch
{
    /// <summary>
    /// Interface for wire adapters
    /// </summary>
    public interface IAdapter : IDisposable
    {
        /// <summary>
        /// Gets the related wire
        /// </summary>
        IWire Wire { get; }

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
        /// Sets a new configuration to the adapter
        /// </summary>
        /// <param name="configuration">serialized configuration</param>
        /// <param name="cancellationToken">cancellation token</param>
        Task SetConfiguration(string configuration, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a call when the current configuration changes.
        /// </summary>
        event Action<IAdapter, string> ConfigurationChanged;

        /// <summary>
        /// Gets a call when the current environment changes.
        /// </summary>
        event Action<IAdapter, string> EnvironmentChanged;
    }
}
