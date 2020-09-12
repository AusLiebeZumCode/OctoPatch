using System;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for incoming messages
    /// </summary>
    internal sealed class InputConnector : Connector, IInputConnector, IInputConnectorHandler
    {
        public InputConnector(Guid nodeId, ConnectorDescription description) 
            : base(nodeId, description)
        {
        }

        public IInputConnectorHandler HandleRaw(Action<Message> handler)
        {
            return this;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IInputConnectorHandler Handle(Action handler)
        {
            return this;
        }
        
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IInputConnectorHandler Handle<T>(Action<T> handler) where T : struct
        {
            return this;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(Message value)
        {
        }
    }
}
