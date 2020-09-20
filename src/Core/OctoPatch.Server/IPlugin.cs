using System;
using System.Collections.Generic;
using OctoPatch.Descriptions;

namespace OctoPatch.Server
{
    /// <summary>
    /// Interface to describe a plugin
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the unique plugin id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the optional description for the plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the current version of this plugin.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Lists up all containing node descriptions.
        /// </summary>
        /// <returns>List of node descriptions</returns>
        IEnumerable<NodeDescription> GetNodeDescriptions();

        /// <summary>
        /// Lists up all containing type descriptions.
        /// </summary>
        /// <returns>List of type descriptions</returns>
        IEnumerable<TypeDescription> GetTypeDescriptions();

        /// <summary>
        /// Lists up all containing adapter descriptions.
        /// </summary>
        /// <returns>List of adapter descriptions</returns>
        IEnumerable<AdapterDescription> GetAdapterDescriptions();

        /// <summary>
        /// Method to generate a new instance of the node with the given key.
        /// </summary>
        /// <param name="key">node description key</param>
        /// <param name="nodeId">id of the new node</param>
        /// <param name="parent">optional reference to the parent node in case of an attached node</param>
        /// <param name="connectorKey">optional key of the connector to attach to</param>
        /// <returns>new instance of a node with the given guid</returns>
        INode CreateNode(string key, Guid nodeId, INode parent = null, string connectorKey = null);

        /// <summary>
        /// Method to generate a new instance of the adapter with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IAdapter CreateAdapter(string key);
    }
}
