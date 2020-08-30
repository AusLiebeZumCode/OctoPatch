using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using OctoPatch.Descriptions;
using OctoPatch.Logging;

namespace OctoPatch.Server
{
    /// <summary>
    /// Implementation for a custom types repository
    /// </summary>
    public sealed class Repository : IRepository
    {
        private static readonly ILogger<Repository> logger = LogManager.GetLogger<Repository>();

        /// <summary>
        /// List of all known plugins
        /// </summary>
        private readonly List<IPlugin> _plugins;

        private readonly Dictionary<string, IPlugin> _nodeMapping;

        private readonly Dictionary<string, IPlugin> _adapterMapping;

        public Repository()
        {
            _plugins = new List<IPlugin>();
            _adapterMapping = new Dictionary<string, IPlugin>();
            _nodeMapping = new Dictionary<string, IPlugin>();

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
                logger?.LogInformation("Checking assembly {FilePath} for Plugins", filename);

                var assembly = Assembly.LoadFrom(filename);

                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsPublic || type.IsAbstract)
                    {
                        continue;
                    }

                    if (typeof(IPlugin).IsAssignableFrom(type))
                    {
                        logger?.LogDebug("Add Plugin '{PluginType}'", type);
                        var plugin = (IPlugin)Activator.CreateInstance(type);
                        _plugins.Add(plugin);

                        foreach (var nodeDescription in plugin.GetNodeDescriptions())
                        {
                            _nodeMapping.Add(nodeDescription.Key, plugin);
                        }

                        foreach (var adapterDescription in plugin.GetAdapterDescriptions())
                        {
                            _adapterMapping.Add(adapterDescription.Key, plugin);
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
        public IEnumerable<TypeDescription> GetTypeDescriptions()
        {
            return _plugins.SelectMany(p => p.GetTypeDescriptions());
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
        public IEnumerable<AdapterDescription> GetAdapterDescriptions()
        {
            return _plugins.SelectMany(p => p.GetAdapterDescriptions());
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public INode CreateNode(string key, Guid nodeId)
        {
            if (!_nodeMapping.TryGetValue(key, out var plugin))
            {
                return null;
            }

            return plugin.CreateNode(key, nodeId);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IAdapter CreateAdapter(string key)
        {
            if (!_adapterMapping.TryGetValue(key, out var plugin))
            {
                return null;
            }

            return plugin.CreateAdapter(key);
        }
    }
}
