namespace OctoPatch
{
    /// <summary>
    /// Represents a single wire between two connectors
    /// </summary>
    internal sealed class Wire : IWire
    {
        private IAdapter _adapter;

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
