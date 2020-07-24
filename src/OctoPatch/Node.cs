using System.Collections.Generic;
using Newtonsoft.Json;
using OctoPatch.Exchange;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of node implementations
    /// </summary>
    /// <typeparam name="T">configuration type</typeparam>
    public abstract class Node<T> : INode where T : INodeConfiguration
    {
        protected readonly List<IInputConnector> _inputs;

        protected readonly List<IOutputConnector> _outputs;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public NodeInstance Instance { get; private set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<IInputConnector> Inputs => _inputs;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<IOutputConnector> Outputs => _outputs;

        protected Node()
        {
            _inputs = new List<IInputConnector>();
            _outputs = new List<IOutputConnector>();
        }

        public void Dispose()
        {
            OnDispose();
        }

        public void Setup(NodeInstance instance)
        {
            Instance = instance;

            var configuration = JsonConvert.DeserializeObject<T>(instance.Configuration);
            Configure(configuration);
        }

        protected abstract void OnDispose();

        public abstract void Configure(T configuration);
    }
}
