using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description of a complex message type
    /// </summary>
    public sealed class TypeDescription : KeyDescription
    {
        /// <summary>
        /// List of containing properties
        /// </summary>
        public List<PropertyDescription> PropertyDescriptions { get; set; }

        public TypeDescription(string key, string displayName, string displayDescription, params PropertyDescription[] propertyDescriptions) 
            : base(key, displayName, displayDescription)
        {
            PropertyDescriptions = propertyDescriptions?.ToList() ?? new List<PropertyDescription>();
        }

        public static TypeDescription Create<T>(Guid pluginId, string displayName, string displayDescription,
            params PropertyDescription[] propertyDescriptions)
        {
            return new TypeDescription($"{pluginId}:{typeof(T).Name}", displayName, displayDescription, propertyDescriptions);
        }
    }
}
