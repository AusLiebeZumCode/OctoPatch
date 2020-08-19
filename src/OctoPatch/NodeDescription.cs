using System;
using System.Collections.Generic;

namespace OctoPatch
{
    /// <summary>
    /// Meta description of a node
    /// </summary>
    public sealed class NodeDescription
    {
        /// <summary>
        /// Id of a node type
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Name of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the node
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// full type name of the node
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Version of the type implementation
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// List of input connectors
        /// </summary>
        public List<InputDescription> InputDescriptions { get; set; }

        /// <summary>
        /// List of output connectors
        /// </summary>
        public List<OutputDescription> OutputDescriptions { get; set; }
    }
}
