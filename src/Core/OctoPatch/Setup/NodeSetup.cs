﻿using System;

namespace OctoPatch.Setup
{
    /// <summary>
    /// Represents an instance of a node within a grid
    /// </summary>
    public sealed class NodeSetup
    {
        /// <summary>
        /// Unique id for this instance
        /// </summary>
        public Guid NodeId { get; set; }

        /// <summary>
        /// Unique key of the node description
        /// </summary>
        public string Key { get; set; }

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
