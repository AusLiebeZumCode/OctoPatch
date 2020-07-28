using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OctoPatch.Exchange;

namespace OctoPatch
{
    /// <summary>
    /// Implementation for a custom types repository
    /// </summary>
    public sealed class Repository : IRepository
    {
        private readonly List<IPlugin> _plugins;

        private readonly Dictionary<Guid, IPlugin> _nodeDescriptions;

        public Repository()
        {
            _plugins = new List<IPlugin>();
            _nodeDescriptions = new Dictionary<Guid, IPlugin>();

            var executablePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            DirectoryInfo directoryInfo = new DirectoryInfo(executablePath);
            foreach (var fileInfo in directoryInfo.GetFiles("*.dll"))
            {
                LoadPluginsFromAssembly(fileInfo.FullName);
            }
        }

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
                            _nodeDescriptions.Add(nodeDescription.Guid, plugin);
                        }
                    }
                }
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<NodeDescription> GetNodeDescriptions()
        {
            throw new NotImplementedException();
        }
    }
}
