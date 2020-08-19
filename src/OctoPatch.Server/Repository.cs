using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Core;

namespace OctoPatch.Server
{
    /// <summary>
    /// Implementation for a custom types repository
    /// </summary>
    public sealed class Repository : IRepository
    {
        /// <summary>
        /// List of all known plugins
        /// </summary>
        private readonly List<IPlugin> _plugins;

        /// <summary>
        /// Mapping from node guid to responsible plugin
        /// </summary>
        private readonly Dictionary<Guid, IPlugin> _nodeToPluginMapping;

        public Repository()
        {
            _plugins = new List<IPlugin>();
            _nodeToPluginMapping = new Dictionary<Guid, IPlugin>();

            var executablePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            DirectoryInfo directoryInfo = new DirectoryInfo(executablePath);
            foreach (var fileInfo in directoryInfo.GetFiles("*.dll"))
            {
                LoadPluginsFromAssembly(fileInfo.FullName);
            }
        }

        /// <summary>
        /// Loads the assembly of the given path and scan it for existing plugins
        /// </summary>
        /// <param name="filename">filename</param>
        private void LoadPluginsFromAssembly(string filename)
        {
            try
            {
                var assembly = Assembly.LoadFrom(filename);

                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsPublic || type.IsAbstract)
                    {
                        continue;
                    }

                    if (typeof(IPlugin).IsAssignableFrom(type))
                    {
                        var plugin = (IPlugin) Activator.CreateInstance(type);

                        _plugins.Add(plugin);
                        foreach (var nodeDescription in plugin.GetNodeDescriptions())
                        {
                            _nodeToPluginMapping.Add(nodeDescription.Guid, plugin);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
                // Ignore reflection issues
            }
            catch (BadImageFormatException)
            {
                // Sad :( This file does not load properly
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<MessageDescription> GetMessageDescriptions()
        {
            return _plugins.SelectMany(p => p.GetMessageDescriptions());
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<NodeDescription> GetNodeDescriptions()
        {
            return _plugins.SelectMany(p => p.GetNodeDescriptions());
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Task<INode> CreateNode(Guid nodeDescriptionGuid, Guid nodeId, CancellationToken cancellationToken)
        {
            if (!_nodeToPluginMapping.TryGetValue(nodeDescriptionGuid, out var plugin))
                return null;

            return plugin.CreateNode(nodeDescriptionGuid, nodeId, cancellationToken);
        }
    }
}
