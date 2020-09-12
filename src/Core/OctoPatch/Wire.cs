using System;

namespace OctoPatch
{
    /// <summary>
    /// Represents a single wire between two connectors
    /// </summary>
    public sealed class Wire : IWire
    {
        private readonly IDisposable _subscription;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IInputConnector Input { get; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IOutputConnector Output { get; }

        public Wire(Guid id, IInputConnector input, IOutputConnector output)
        {
            Id = id;
            Input = input;
            Output = output;

            _subscription = output.Subscribe(input);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
