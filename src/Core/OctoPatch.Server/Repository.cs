using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;

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

        public Repository()
        {
            _plugins = new List<IPlugin>();

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
        public IEnumerable<TypeDescription> GetMessageDescriptions()
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
        public Task<INode> CreateNode(Guid pluginId, string key, Guid nodeId, CancellationToken cancellationToken)
        {
            var plugin = _plugins.FirstOrDefault(p => p.Id == pluginId);
            if (plugin == null)
            {
                return Task.FromResult<INode>(null);
            }

            return plugin.CreateNode(key, nodeId, cancellationToken);
        }
    }
}
