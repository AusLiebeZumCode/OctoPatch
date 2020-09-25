using System;
using System.Runtime.Serialization;

namespace OctoPatch.Setup
{
    /// <summary>
    /// Represents an instance of a node within a grid
    /// </summary>
    [DataContract]
    public sealed class NodeSetup
    {
        /// <summary>
        /// Unique id for this instance
        /// </summary>
        [DataMember]
        public Guid NodeId { get; set; }

        /// <summary>
        /// Unique key of the node description
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Gets the id of an optional parent.
        /// This is used for attached nodes to point to the parent node
        /// </summary>
        [DataMember]
        public Guid? ParentNodeId { get; set; }

        /// <summary>
        /// Gets or sets the optional connector key.
        /// This is used for splitter and collector to point to the related output
        /// </summary>
        [DataMember]
        public string ParentConnector { get; set; }

        /// <summary>
        /// Name of this instance
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Optional description for this instance
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Configuration string for this instance
        /// </summary>
        [DataMember]
        public string Configuration { get; set; }

        /// <summary>
        /// X Position for this node within the grid
        /// </summary>
        [DataMember]
        public int PositionX { get; set; }

        /// <summary>
        /// Y Position for this node within the grid
        /// </summary>
        [DataMember]
        public int PositionY { get; set; }
    }
}
