using System.Collections.Generic;

namespace OctoPatch
{
    /// <summary>
    /// Represents a single grid setup
    /// </summary>
    public sealed class Grid
    {
        /// <summary>
        /// Name of the grid
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional description for this grid
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of node instances
        /// </summary>
        public List<NodeInstance> NodeInstances { get; set; }

        /// <summary>
        /// List of wire instances
        /// </summary>
        public List<WireInstance> WireInstances { get; set; }
    }
}
