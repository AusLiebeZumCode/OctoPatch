using System;

namespace OctoPatch.Core
{
    public sealed class InputConnector : IInputConnector
    {
        public Guid Guid { get; }

        public InputConnector(Guid guid)
        {
            Guid = guid;
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
