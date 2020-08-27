using System;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for incoming messages
    /// </summary>
    public sealed class InputConnector : Connector, IInputConnector
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

        public IInputConnector HandleEmpty(Action handler)
        {
            return this;
        }

        public IInputConnector HandleBool(Action<bool> handler)
        {
            return this;
        }
        
        public IInputConnector HandleInteger(Action<int> handler)
        {
            return this;
        }

        public IInputConnector HandleFloat(Action<float> handler)
        {
            return this;
        }
        
        public IInputConnector HandleString(Action<string> handler)
        {
            return this;
        }
        
        public IInputConnector HandleBinary(Action<byte[]> handler)
        {
            return this;
        }
        
        public IInputConnector HandleComplex<T>(Action<T> handler)
        {
            return this;
        }
    }
}
