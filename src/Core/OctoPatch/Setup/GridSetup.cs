using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OctoPatch.Setup
{
    /// <summary>
    /// Represents a single grid setup
    /// </summary>
    [DataContract]
    public sealed class GridSetup
    {
        /// <summary>
        /// Name of the grid
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Optional description for this grid
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// List of node instances
        /// </summary>
        [DataMember]
        public List<NodeSetup> NodeInstances { get; set; }

        /// <summary>
        /// List of wire instances
        /// </summary>
        [DataMember]
        public List<WireSetup> WireInstances { get; set; }
    }
}
