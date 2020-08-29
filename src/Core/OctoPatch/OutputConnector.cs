using System;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for outgoing messages
    /// </summary>
    internal sealed class OutputConnector : Connector, IOutputConnector, IOutputConnectorHandler
    {
        public OutputConnector(ConnectorDescription description) : base(description)
        {
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
