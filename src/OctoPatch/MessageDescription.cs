using System.Collections.Generic;

namespace OctoPatch
{
    /// <summary>
    /// Description of a custom message type
    /// </summary>
    public sealed class MessageDescription
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
    }
}
