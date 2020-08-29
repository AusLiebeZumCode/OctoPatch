using System;
using System.Collections.Generic;
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

        private readonly Dictionary<string, TypeDescription> _splitterMapping;

        private readonly Dictionary<string, TypeDescription> _collectorMapping;

        private readonly Dictionary<string, Type> _nodeTypeMapping;

        private readonly Dictionary<string, Type> _adapterTypeMapping;

        protected Plugin()
        {
            _nodeDescriptions = new List<NodeDescription>();
            _typeDescriptions = new List<TypeDescription>();
            _adapterDescriptions = new List<AdapterDescription>();
            _nodeTypeMapping = new Dictionary<string, Type>();
            _adapterTypeMapping = new Dictionary<string, Type>();
            _splitterMapping = new Dictionary<string, TypeDescription>();
            _collectorMapping = new Dictionary<string, TypeDescription>();
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
        protected void RegisterType<T>(TypeDescription description) where T : struct
        {
            _typeDescriptions.Add(description);

            // Auto generate splitter
            var splitter = SplitterNodeDescription.CreateFromComplexType(description);
            _nodeDescriptions.Add(splitter);
            _splitterMapping.Add(splitter.Key, description);

            // Auto generate collector
            var collector = CollectorNodeDescription.CreateFromComplexType(description);
            _nodeDescriptions.Add(collector);
            _collectorMapping.Add(collector.Key, description);
        }

        /// <summary>
        /// Registers the given adapter at the plugin
        /// </summary>
        /// <param name="description">adapter description</param>
        protected void RegisterAdapter<T>(AdapterDescription description) where T : IAdapter
        {
            _adapterDescriptions.Add(description);
            _adapterTypeMapping.Add(description.Key, typeof(T));
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

        public INode CreateNode(string key, Guid nodeId)
        {
            // Special case of splitter
            if (_splitterMapping.TryGetValue(key, out var splitterTypeDescription))
            {
                return new SplitterNode(nodeId, splitterTypeDescription);
            }

            // Special case of collector
            if (_collectorMapping.TryGetValue(key, out var collectorTypeDescription))
            {
                return new CollectorNode(nodeId, collectorTypeDescription);
            }

            // Common nodes
            if (!_nodeTypeMapping.TryGetValue(key, out var type))
            {
                return null;
            }

            return OnCreateNode(type, nodeId);
        }

        /// <summary>
        /// Gets a call when the given node was requested
        /// </summary>
        /// <param name="type">requested node type</param>
        /// <param name="nodeId">node id</param>
        /// <returns>new instance</returns>
        protected abstract INode OnCreateNode(Type type, Guid nodeId);

        public IAdapter CreateAdapter(string key)
        {
            if (!_adapterTypeMapping.TryGetValue(key, out var type))
            {
                return null;
            }

            return OnCreateAdapter(type);
        }

        /// <summary>
        /// Gets a call when the given adapter was requested
        /// </summary>
        /// <param name="type">requested adapter type</param>
        /// <returns>new instance</returns>
        protected abstract IAdapter OnCreateAdapter(Type type);
    }
}
