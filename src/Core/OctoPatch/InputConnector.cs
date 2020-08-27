using System;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for incoming messages
    /// </summary>
    public sealed class InputConnector : Connector, IInputConnector, IInputConnectorHandler
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
        public IInputConnectorHandler Handle<T>(Action<T> handler)
        {
            return this;
        }
    }
}
