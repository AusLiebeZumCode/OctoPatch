using OctoPatch.Communication;

namespace OctoPatch
{
    /// <summary>
    /// Represents a single wire between two connectors
    /// </summary>
    public sealed class Wire : IWire
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public WireInstance Instance { get; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IInputConnector InputConnector { get; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IOutputConnector OutputConnector { get; }

        public Wire(WireInstance instance, IInputConnector input, IOutputConnector output)
        {
            Instance = instance;
            InputConnector = input;
            OutputConnector = output;
        }
    }
}
