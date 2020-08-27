using System;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for outgoing messages
    /// </summary>
    public sealed class OutputConnector : Connector, IOutputConnector, IOutputConnectorHandler
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public OutputDescription OutputDescription { get; }

        public OutputConnector(OutputDescription outputDescription) 
            : base(outputDescription?.Guid ?? throw new ArgumentNullException(nameof(outputDescription)))
        {
            OutputDescription = outputDescription;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Send()
        {
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Send<T>(T value)
        {
        }
    }
}
