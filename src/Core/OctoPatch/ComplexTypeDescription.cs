using System.Collections.Generic;
using System.Linq;

namespace OctoPatch
{
    /// <summary>
    /// Description of a complex message type
    /// </summary>
    public sealed class ComplexTypeDescription
    {
        /// <summary>
        /// Name of this custom message
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of this custom message
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of containing properties
        /// </summary>
        public List<PropertyDescription> PropertyDescriptions { get; set; }

        public ComplexTypeDescription()
        {

        }

        public ComplexTypeDescription(string name, string description, params PropertyDescription[] propertyDescriptions)
        {
            Name = name;
            Description = description;
            PropertyDescriptions = propertyDescriptions?.ToList() ?? new List<PropertyDescription>();
        }
    }
}
