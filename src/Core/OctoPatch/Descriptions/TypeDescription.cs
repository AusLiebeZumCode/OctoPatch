using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description of a complex message type
    /// </summary>
    public sealed class TypeDescription : Description
    {
        /// <summary>
        /// Gets or sets the id of the related plugin
        /// </summary>
        public Guid PluginId { get; set; }

        /// <summary>
        /// Key for this type. Usually the type name (without namespace)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// List of containing properties
        /// </summary>
        public List<PropertyDescription> PropertyDescriptions { get; set; }

        public TypeDescription() { }

        public TypeDescription(Guid pluginId, string key, string displayName, string displayDescription, params PropertyDescription[] propertyDescriptions) 
            : base(displayName, displayDescription)
        {
            PluginId = pluginId;
            Key = key;
            PropertyDescriptions = propertyDescriptions?.ToList() ?? new List<PropertyDescription>();
        }
    }
}
