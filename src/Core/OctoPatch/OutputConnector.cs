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
            Send(message);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void SendRaw(Message message)
        {
            Send(message);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Send<T>(T value) where T : struct
        {
            var message = Message.Create(value);
            Send(message);
        }

        private void InternalSend(Message message)
        {

        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IDisposable Subscribe(IObserver<Message> observer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IDisposable Subscribe<T>(Action<T> messageHandler) where T : struct
        {
            return this.Subscribe((m) =>
            {
                // Handle message if it's from given type
                if (m.Content is T specificType)
                {
                    messageHandler(specificType);
                }
            });
        }
    }
}
