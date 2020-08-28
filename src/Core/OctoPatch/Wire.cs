using System;

namespace OctoPatch
{
    /// <summary>
    /// Represents a single wire between two connectors
    /// </summary>
    internal sealed class Wire : IWire
    {
        private readonly IDisposable _subscription;

        public Wire(IInputConnector input, IOutputConnector output)
        {
            _subscription = output.Subscribe(input);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
