using System;

namespace OctoPatch
{
    /// <summary>
    /// Represents an instance of a node within a grid
    /// </summary>
    public sealed class NodeInstance
    {
        /// <summary>
        /// Unique id for this instance
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Id of the node definition
        /// </summary>
        public Guid NodeDescription { get; set; }

        /// <summary>
        /// Name of this instance
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional description for this instance
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Configuration string for this instance
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// X Position for this node within the grid
        /// </summary>
        public int PositionX { get; set; }

        /// <summary>
        /// Y Position for this node within the grid
        /// </summary>
        public int PositionY { get; set; }
    }
}
