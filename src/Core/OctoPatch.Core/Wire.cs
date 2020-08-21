namespace OctoPatch.Core
{
    /// <summary>
    /// Represents a single wire between two connectors
    /// </summary>
    public sealed class Wire : IWire
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IInputConnector InputConnector { get; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IOutputConnector OutputConnector { get; }

        public Wire(IInputConnector input, IOutputConnector output)
        {
            InputConnector = input;
            OutputConnector = output;
        }
    }
}
