using System;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for incoming messages
    /// </summary>
    internal sealed class InputConnector : Connector, IInputConnector, IInputConnectorHandler
    {
        /// <summary>
        /// Returns the description for this input
        /// </summary>
        public InputDescription InputDescription { get; }

        public InputConnector(InputDescription inputDescription) 
            : base(inputDescription?.Guid ?? throw new ArgumentNullException(nameof(inputDescription)))
        {
            InputDescription = inputDescription;
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
