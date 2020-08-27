using System;

namespace OctoPatch
{
    /// <summary>
    /// Represents a single message
    /// </summary>
    public struct Message
    {
        /// <summary>
        /// Gets or sets the type of the serialized message
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public ValueType Content { get; set; }
    }
}
