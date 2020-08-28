using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;

namespace OctoPatch.Server
{
    /// <summary>
    /// Base class for all plugins
    /// </summary>
    public abstract class Plugin : IPlugin
    {
        public abstract Guid Id { get; }

        public abstract string Name { get; }
        
        public abstract string Description { get; }
        
        public abstract Version Version { get; }

        private readonly List<NodeDescription> _nodeDescriptions;

        private readonly List<TypeDescription> _typeDescriptions;

        private readonly List<AdapterDescription> _adapterDescriptions;

        private readonly Dictionary<string, Type> _nodeTypeMapping;

        protected Plugin()
        {
            _nodeDescriptions = new List<NodeDescription>();
            _typeDescriptions = new List<TypeDescription>();
            _adapterDescriptions = new List<AdapterDescription>();
            _nodeTypeMapping = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Registers the given node at the plugin
        /// </summary>
        /// <param name="description">node description</param>
        protected void RegisterNode<T>(NodeDescription description) where T : INode
        {
            _nodeDescriptions.Add(description);
            _nodeTypeMapping.Add(description.Key, typeof(T));
        }

        /// <summary>
        /// Registers the given type at the plugin
        /// </summary>
        /// <param name="description">type description</param>
        protected void RegisterType(TypeDescription description)
        {
            _typeDescriptions.Add(description);
        }

        /// <summary>
        /// Registers the given adapter at the plugin
        /// </summary>
        /// <param name="description">adapter description</param>
        protected void RegisterAdapter(AdapterDescription description)
        {
            _adapterDescriptions.Add(description);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<NodeDescription> GetNodeDescriptions() => _nodeDescriptions;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<TypeDescription> GetTypeDescriptions() => _typeDescriptions;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<AdapterDescription> GetAdapterDescriptions() => _adapterDescriptions;

        public Task<INode> CreateNode(string key, Guid nodeId, CancellationToken cancellationToken)
        {
            if (!_nodeTypeMapping.TryGetValue(key, out var type))
            {
                return Task.FromResult<INode>(null);
            }

            return OnCreateNode(type, nodeId, cancellationToken);
        }

        /// <summary>
        /// Gets a call when the given type was requested
        /// </summary>
        /// <param name="type">requested node type</param>
        /// <param name="nodeId">node id</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>new instance</returns>
        protected abstract Task<INode> OnCreateNode(Type type, Guid nodeId, CancellationToken cancellationToken);
    }
}
