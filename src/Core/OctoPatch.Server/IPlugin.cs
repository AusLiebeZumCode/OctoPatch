using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        /// Method to generate a new instance of the node with the given node guid.
        /// </summary>
        /// <param name="key">node description key</param>
        /// <param name="nodeId">id of the new node</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>new instance of a node with the given guid</returns>
        Task<INode> CreateNode(string key, Guid nodeId, CancellationToken cancellationToken);

    }
}
