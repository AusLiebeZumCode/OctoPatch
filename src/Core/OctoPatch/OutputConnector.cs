using System;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for outgoing messages
    /// </summary>
    internal sealed class OutputConnector : Connector, IOutputConnector, IOutputConnectorHandler
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
            var message = Message.Create();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Send<T>(T value) where T : struct
        {
            var message = Message.Create(value);
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            throw new NotImplementedException();
        }
    }
}
