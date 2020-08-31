using Microsoft.Extensions.Logging;
using OctoPatch.Logging;
using System;
using System.Collections.Generic;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Meta description of a node
    /// </summary>
    public class NodeDescription : KeyDescription
    {
        private static readonly ILogger<NodeDescription> logger = LogManager.GetLogger<NodeDescription>();

        /// <summary>
        /// Returns the actual type of the node description
        /// </summary>
        public string NodeType => GetType().Name;

        /// <summary>
        /// List of input connectors
        /// </summary>
        public List<ConnectorDescription> InputDescriptions { get; set; }

        /// <summary>
        /// List of output connectors
        /// </summary>
        public List<ConnectorDescription> OutputDescriptions { get; set; }

        protected NodeDescription(string key, string displayName, string displayDescription)
            : base(key, displayName, displayDescription)
        {
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
            logger.LogDebug("Add new output description '{Key} - {DisplayName}'", description.Key, description.DisplayName, description);
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
            logger.LogDebug("Add new input description '{Key} - {DisplayName}'", description.Key, description.DisplayName, description);
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
            logger.LogDebug("Create new node description for type {Type} in plugin {PluginId}", typeof(T), pluginId);
            return new NodeDescription($"{pluginId}:{typeof(T).Name}", displayName, displayDescription);
        }
    }
}
