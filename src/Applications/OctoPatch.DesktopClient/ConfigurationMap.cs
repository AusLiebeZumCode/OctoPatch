using System;
using System.Collections.Generic;
using System.Linq;
using OctoPatch.DesktopClient.Models;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

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
                            Map.Add(new MapEntry(attribute.Key, attribute.ModelType, type));
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
        /// Returns a new instance of the configuration model fitting to the given key or null, when no key is registered
        /// </summary>
        /// <param name="key">node key</param>
        /// <returns>new model instance</returns>
        public static ConfigurationModel GetConfigurationModel(string key)
        {
            var entry = Map.FirstOrDefault(e => string.Equals(e.Key, key, StringComparison.InvariantCultureIgnoreCase));
            return entry == null ? null : (ConfigurationModel)Activator.CreateInstance(entry.ModelType);
        }

        public static UserControl GetConfigurationView(ConfigurationModel model)
        {
            if (model == null)
            {
                return null;
            }

            var entry = Map.FirstOrDefault(e => e.ModelType == model.GetType());
            if (entry == null)
            {
                return null;
            }

            var control = (UserControl) Activator.CreateInstance(entry.ViewType);

            // Set data context if possible
            if (control != null)
            {
                control.DataContext = model;
            }

            return control;
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
            public Type ModelType { get; }

            /// <summary>
            /// Gets the type of the view
            /// </summary>
            public Type ViewType { get; }

            /// <summary>
            /// Gets the key of the node or adapter
            /// </summary>
            public string Key { get; }

            public MapEntry(string key, Type modelType, Type viewType)
            {
                Key = key;
                ModelType = modelType;
                ViewType = viewType;
            }
        }

        #endregion
    }
}
