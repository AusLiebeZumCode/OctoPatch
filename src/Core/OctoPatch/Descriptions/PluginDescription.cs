using System;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Represents a single plugin
    /// </summary>
    public sealed class PluginDescription : Description
    {
        /// <summary>
        /// Gets or sets the unique id for this plugin
        /// </summary>
        public Guid Id { get; set; }

        public PluginDescription(Guid id, string displayName, string displayDescription) 
            : base(displayName, displayDescription)
        {
            Id = id;
        }
    }
}
