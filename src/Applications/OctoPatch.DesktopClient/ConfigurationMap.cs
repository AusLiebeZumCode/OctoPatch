using System;
using System.Collections.Generic;
using System.Linq;
using OctoPatch.DesktopClient.Models;
using System.Reflection;

namespace OctoPatch.DesktopClient
{
    /// <summary>
    /// Central place to map configuration setup (node/adapter key to configuration model and configuration view)
    /// </summary>
    public sealed class ConfigurationMap
    {
        /// <summary>
        /// Holds the current map
        /// </summary>
        private static readonly List<MapEntry> Map = new List<MapEntry>();

        /// <summary>
        /// Constructor scans for existing configuration map attributes within the current app domain
        /// </summary>
        static ConfigurationMap()
        {
            // Scan all referenced assemblies
            foreach (var assemblies in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // Scan all types
                    foreach (var type in assemblies.GetTypes())
                    {
                        // Scan all existing attributes
                        foreach (var attribute in type.GetCustomAttributes<ConfigurationMapAttribute>())
                        {
                            Map.Add(new MapEntry
                            {
                                ViewType = type,
                                ModelType = attribute.ModelType,
                                Key = attribute.Key
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    // Some assemblies are not allowed to be reflected...?
                }
            }
        }

        /// <summary>
        /// Returns a new instance of the node configuration model fitting to the given key or null, when no key is registered
        /// </summary>
        /// <param name="key">node key</param>
        /// <returns>new model instance</returns>
        public static NodeConfigurationModel GetNodeConfigurationModel(string key)
        {
            return GetConfigurationModel<NodeConfigurationModel>(key);
        }

        /// <summary>
        /// Returns a new instance of the adapter configuration model fitting to the given key or null, when no key is registered
        /// </summary>
        /// <param name="key">adapter key</param>
        /// <returns>new model instance</returns>
        public static AdapterConfigurationModel GetAdapterConfigurationModel(string key)
        {
            return GetConfigurationModel<AdapterConfigurationModel>(key);
        }

        /// <summary>
        /// Generates an instance of the registered model for the given key
        /// </summary>
        /// <typeparam name="T">type of configuration model</typeparam>
        /// <param name="key">node or adapter key</param>
        /// <returns>new instance of the given type</returns>
        private static T GetConfigurationModel<T>(string key) where T : Model
        {
            var entry = Map.FirstOrDefault(e => string.Equals(e.Key, key, StringComparison.InvariantCultureIgnoreCase));
            return entry == null ? null : (T)Activator.CreateInstance(entry.ModelType);
        }

        #region nested types

        /// <summary>
        /// Local container to store a map entry
        /// </summary>
        private class MapEntry
        {
            /// <summary>
            /// Gets the type of the data model
            /// </summary>
            public Type ModelType { get; set; }

            /// <summary>
            /// Gets the type of the view
            /// </summary>
            public Type ViewType { get; set; }

            /// <summary>
            /// Gets the key of the node or adapter
            /// </summary>
            public string Key { get; set; }
        }

        #endregion
    }
}
