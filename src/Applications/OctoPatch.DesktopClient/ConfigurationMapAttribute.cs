using System;

namespace OctoPatch.DesktopClient
{
    /// <summary>
    /// Attribute to decorate the configuration view with
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ConfigurationMapAttribute : Attribute
    {
        /// <summary>
        /// Gets the key string for the node or adapter
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the model type which handles the node or adapter settings
        /// </summary>
        public Type ModelType { get; }

        public ConfigurationMapAttribute(string key, Type modelType)
        {
            Key = key;
            ModelType = modelType;
        }
    }
}
