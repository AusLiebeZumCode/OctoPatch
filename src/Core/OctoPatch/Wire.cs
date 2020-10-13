using System;

namespace OctoPatch
{
    /// <summary>
    /// Represents a single wire between two connectors
    /// </summary>
    public sealed class Wire : IWire
    {
        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public IInputConnector Output { get; }

        /// <inheritdoc />
        public IOutputConnector Input { get; }

        public Wire(Guid id, IOutputConnector input, IInputConnector output)
        {
            Id = id;
            Input = input;
            Output = output;
        }
    }
}
