using System;
using System.Collections.Generic;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Meta description of a node
    /// </summary>
    public class NodeDescription : Description
    {
        /// <summary>
        /// Returns the actual type of the node description
        /// </summary>
        public string NodeType => GetType().Name;

        /// <summary>
        /// Gets or sets the id of the related plugin
        /// </summary>
        public Guid PluginId { get; set; }

        /// <summary>
        /// Key for this type. Usually the type name (without namespace)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// List of input connectors
        /// </summary>
        public List<ConnectorDescription> InputDescriptions { get; set; }

        /// <summary>
        /// List of output connectors
        /// </summary>
        public List<ConnectorDescription> OutputDescriptions { get; set; }

        protected NodeDescription() { }

        protected NodeDescription(Guid pluginId, string key, string displayName, string displayDescription) 
            : base(displayName, displayDescription)
        {
            PluginId = pluginId;
            Key = key;
            InputDescriptions = new List<ConnectorDescription>();
            OutputDescriptions = new List<ConnectorDescription>();
        }

        /// <summary>
        /// Adds an output description to the node
        /// </summary>
        /// <param name="description">description</param>
        /// <returns>current instance to chain adds</returns>
        public NodeDescription AddOutputDescription(ConnectorDescription description)
        {
            OutputDescriptions.Add(description);
            return this;
        }

        /// <summary>
        /// Adds an input description to the node
        /// </summary>
        /// <param name="description">description</param>
        /// <returns>current instance to chain adds</returns>
        public NodeDescription AddInputDescription(ConnectorDescription description)
        {
            InputDescriptions.Add(description);
            return this;
        }

        /// <summary>
        /// Creates a new description for common nodes
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="pluginId">plugin id</param>
        /// <param name="displayName">name of the node</param>
        /// <param name="displayDescription">optional description</param>
        /// <returns>node description</returns>
        public static NodeDescription Create<T>(Guid pluginId, string displayName, string displayDescription)
        {
            return new NodeDescription(pluginId, typeof(T).Name, displayName, displayDescription);
        }
    }
}
