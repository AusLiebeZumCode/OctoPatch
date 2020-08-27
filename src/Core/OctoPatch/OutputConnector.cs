using System;

namespace OctoPatch
{
    /// <summary>
    /// Connector implementation for outgoing messages
    /// </summary>
    public sealed class OutputConnector : Connector, IOutputConnector
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

        public void SendEmpty()
        {

        }

        public void SendInteger(int value)
        {

        }

        public void SendFloat(float value)
        {

        }

        public void SendBool(bool value)
        {

        }

        public void SendString(string value)
        {

        }

        public void SendBinary(byte[] value)
        {

        }

        public void SendComplex<T>(T value)
        {

        }
    }
}
